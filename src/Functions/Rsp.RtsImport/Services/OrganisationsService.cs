using System.Collections.Concurrent;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Refit;
using Rsp.Logging.Extensions;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Application.DTO;
using Rsp.RtsImport.Application.DTO.Responses;
using Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;
using Rsp.RtsImport.Application.ServiceClients;
using Rsp.RtsImport.Application.Settings;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;

namespace Rsp.RtsImport.Services;

public class OrganisationsService
(
    IRtsServiceClient rtsClient,
    RtsDbContext db,
    AppSettings appSettings,
    ILogger<OrganisationsService> logger
) : IOrganisationService
{
    public async Task<DbOperationResult> UpdateOrganisations
    (
        IEnumerable<Organisation> items,
        bool onlyActive = false
    )
    {
        var result = new DbOperationResult();

        if (onlyActive)
        {
            items = items.Where(x => x.Status == RtsRecordStatusOptions.ActiveOrg);
        }

        await using var trans = await db.Database.BeginTransactionAsync();

        db.Database.SetCommandTimeout(appSettings.DatabaseCommandTimeout);

        var bulkConfig = new BulkConfig
        {
            UseTempDB = true,
            SetOutputIdentity = false,
            ConflictOption = ConflictOption.None,
            BatchSize = 10000,
            TrackingEntities = false,
            CalculateStats = true,
            BulkCopyTimeout = appSettings.BulkCopyTimeout
        };
        await db.BulkInsertOrUpdateAsync(items, bulkConfig);
        await trans.CommitAsync();

        if (bulkConfig.StatsInfo != null)
        {
            // log number of items updated/inserted to the DB
            result.RecordsUpdated =
                bulkConfig.StatsInfo.StatsNumberUpdated + bulkConfig.StatsInfo.StatsNumberInserted;
        }

        return result;
    }

    public async Task<DbOperationResult> UpdateRoles
    (
        IEnumerable<OrganisationRole> items,
        bool onlyActive = false
    )
    {
        if (onlyActive)
        {
            items = items.Where(x => x.Status == RtsRecordStatusOptions.ActiveRole);
        }

        var result = new DbOperationResult();

        await using var trans = await db.Database.BeginTransactionAsync();
        // Use HashSet for O(1) lookup
        var organisationIds = new HashSet<string>(
            await db.Organisation.Select(x => x.Id).ToListAsync()
        );

        // This is now super fast (O(n))
        var filteredRoles = items
            .Where(x => organisationIds.Contains(x.OrganisationId))
            .ToList(); // Materialize only once for BulkInsertOrUpdateAsync

        if (!filteredRoles.Any())
        {
            result.RecordsUpdated = 0;
            return result;
        }

        db.Database.SetCommandTimeout(appSettings.DatabaseCommandTimeout);

        var bulkConfig = new BulkConfig
        {
            UseTempDB = true,
            SetOutputIdentity = false,
            ConflictOption = ConflictOption.Replace, // Ensure both insert and update
            BatchSize = 10000,
            CalculateStats = true,
            BulkCopyTimeout = appSettings.BulkCopyTimeout
        };

        await db.BulkInsertOrUpdateAsync(filteredRoles, bulkConfig);
        await trans.CommitAsync();

        if (bulkConfig.StatsInfo != null)
        {
            result.RecordsUpdated =
                bulkConfig.StatsInfo.StatsNumberUpdated + bulkConfig.StatsInfo.StatsNumberInserted;
        }

        return result;
    }

    public async Task<IEnumerable<RtsOrganisationAndRole>> GetOrganisationsAndRoles(string lastUpdated)
    {
        int pageSize = appSettings.ApiRequestPageSize != 0 ? appSettings.ApiRequestPageSize : 100;
        int maxConcurrency = appSettings.ApiRequestMaxConcurrency != 0 ? appSettings.ApiRequestMaxConcurrency : 8;

        var result = new ConcurrentBag<RtsOrganisationAndRole>();
        var totalRecords = await FetchPageCountAsync(lastUpdated);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        var pageIndices = Enumerable.Range(0, totalPages);

        await Parallel.ForEachAsync(pageIndices, new ParallelOptions { MaxDegreeOfParallelism = maxConcurrency },
            async (page, _) =>
            {
                var offset = page * pageSize;
                var data = await FetchOrganisationAndRolesAsync(lastUpdated, offset, pageSize);
                foreach (var item in data)
                {
                    result.Add(item);
                }
            });

        var finalResult = result
            .DistinctBy(x => new { x.RtsOrganisation.Id, rtsRole = x.RtsRole })
            .ToList();

        return finalResult;
    }

    public async Task<int> FetchPageCountAsync(string lastUpdated)
    {
        ApiResponse<RtsOrganisationsAndRolesResponse>? result =
            await rtsClient.GetOrganisationsAndRoles(lastUpdated, 0, 1);

        return result?.Content?.Total ?? -1;
    }

    public RtsOrganisationAndRole TransformOrganisationAndRoles(RtsFhirEntry entry)
    {
        var rtsOrganisationRole = new RtsOrganisationAndRole();
        try
        {
            var rtsOrganisation = new Organisation
            {
                Id = entry.Resource.Id,
                OId = entry.Resource.Identifier[0].Value,
                Imported = DateTime.Now,
                LastUpdated = entry.Resource.Meta.LastUpdated,
                SystemUpdated = DateTime.Now,
                Status = entry.Resource.Active,
                Address = entry.Resource.Address[0].Text,
                CountryName = entry.Resource.Address[0].Country,
                Name = entry.Resource.Name,
                TypeId = entry.Resource.Type[0].Coding[0].Code,
                TypeName = entry.Resource.Type[0].Text,
                Type = entry.Resource.Type[0].Text
            };

            rtsOrganisationRole.RtsOrganisation = rtsOrganisation;
            rtsOrganisationRole.RtsRole = [];
            var extensions = entry.Resource.Extension;

            var subExtensions = extensions
                .Where(extension => extension.Extension is { Count: > 0 })
                .Select(extension => extension.Extension);
            foreach (var roleExtensions in subExtensions)
            {
                DateTime startDate = DateTime.MinValue;
                var status = "Active";
                var identifier = "";
                var roleName = "";
                var scoper = -1;

                foreach (var roleExtension in roleExtensions)
                {
                    switch (roleExtension.Url)
                    {
                        case "startDate":
                            startDate = Convert.ToDateTime(roleExtension.ValueDate);
                            break;

                        case "status":
                            status = roleExtension.ValueString;
                            break;

                        case "identifier":
                            identifier = roleExtension.ValueString;
                            break;

                        case "name":
                            roleName = roleExtension.ValueString;
                            break;

                        case "scoper":
                            {
                                // Split the URL path by '/'
                                var segments = roleExtension.ValueReference.Reference.Split("/");

                                // Extract the last segment, which is the ID
                                scoper = int.Parse(segments[^1]);
                                break;
                            }
                    }
                }

                var rtsRole = new OrganisationRole
                {
                    Id = identifier,
                    OrganisationId = entry.Resource.Id,
                    Imported = DateTime.Now,
                    StartDate = startDate,
                    SystemUpdated = DateTime.Now,
                    Scoper = scoper,
                    Status = status,
                    RoleName = roleName
                };

                // api can contain duplicate data so filter duplicates out here
                if (rtsOrganisationRole.RtsRole.FirstOrDefault(
                    x => x.Id == rtsRole.Id &&
                    x.Scoper == rtsRole.Scoper &&
                    x.OrganisationId == rtsRole.OrganisationId &&
                    x.Status == rtsRole.Status &&
                    x.StartDate == rtsRole.StartDate) == null)
                {
                    rtsOrganisationRole.RtsRole.Add(rtsRole);
                }
            }

            rtsOrganisationRole.RtsOrganisation.Roles = rtsOrganisationRole.RtsRole;
            return rtsOrganisationRole;
        }
        catch (Exception ex)
        {
            logger.LogAsInformation($"Error: {ex}");
            return rtsOrganisationRole;
        }
    }

    public async Task<IEnumerable<RtsOrganisationAndRole>> FetchOrganisationAndRolesAsync
    (
        string lastUpdated,
        int offset,
        int count
    )
    {
        try
        {
            var result = await rtsClient.GetOrganisationsAndRoles(lastUpdated, offset, count);

            if (result.IsSuccessful)
            {
                var allData = result?.Content?.Entry
                    ?.Select(x => TransformOrganisationAndRoles(x))
                    .ToList();

                return allData ?? [];
            }

            var exception = result.Error;

            logger.LogAsError(exception.StatusCode.ToString(), $"Error fetching data at offset {offset}: {exception.Message}");
            return [];
        }
        catch (Exception ex)
        {
            // Optional: Log and rethrow or return empty on failure
            logger.LogAsError("RTS_API_ERROR", $"Error fetching data at offset {offset}: {ex.Message}");
            return [];
        }
    }
}