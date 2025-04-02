using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;
using Rsp.Logging.Extensions;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Application.DTO;
using Rsp.RtsImport.Application.DTO.Responses;
using Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;
using Rsp.RtsImport.Application.ServiceClients;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;

namespace Rsp.RtsImport.Services;

public class OrganisationsService(IRtsServiceClient rtsClient, RtsDbContext db, ILogger<OrganisationsService> logger) : IOrganisationService
{
    private readonly IRtsServiceClient _rtsClient = rtsClient;
    private readonly RtsDbContext _db = db;
    private readonly ILogger<OrganisationsService> _logger;

    public async Task<DbOperationResult> UpdateOrganisations(IEnumerable<Organisation> dbRecords, bool onlyActive = false)
    {
        var result = new DbOperationResult();

        if (onlyActive)
        {
            dbRecords = dbRecords.Where(x => x.Status == RtsRecordStatusOptions.ActiveOrg);
        }

        using (var trans = await _db.Database.BeginTransactionAsync())
        {
            _db.Database.SetCommandTimeout(500);
            var bulkConfig = new BulkConfig
            {
                UseTempDB = true,
                SetOutputIdentity = false,
                ConflictOption = EFCore.BulkExtensions.ConflictOption.None,
                BatchSize = 10000,
                TrackingEntities = false,
                CalculateStats = true,
                BulkCopyTimeout = 300
            };
            await _db.BulkInsertOrUpdateAsync(dbRecords, bulkConfig);
            await trans.CommitAsync();

            if (bulkConfig.StatsInfo != null)
            {
                // log number of items updated/inserted to the DB
                result.RecordsUpdated = bulkConfig.StatsInfo.StatsNumberUpdated + bulkConfig.StatsInfo.StatsNumberInserted;
            }
        }

        return result;
    }

    public async Task<DbOperationResult> UpdateRoles(IEnumerable<OrganisationRole> dbRecords, bool onlyActive = false)
    {
        if (onlyActive)
        {
            dbRecords = dbRecords.Where(x => x.Status == RtsRecordStatusOptions.ActiveRole);
        }
        var result = new DbOperationResult();
        using (var trans = await _db.Database.BeginTransactionAsync())
        {
            var organisationIds = _db.Organisation.Select(x => x.Id).ToHashSet();
            var filteredRoles = dbRecords.Where(x => organisationIds.Contains(x.OrganisationId));

            _db.Database.SetCommandTimeout(500);
            var bulkConfig = new BulkConfig
            {
                UseTempDB = true,
                SetOutputIdentity = false,
                ConflictOption = EFCore.BulkExtensions.ConflictOption.Ignore,
                BatchSize = 10000,
                CalculateStats = true,
                BulkCopyTimeout = 300
            };
            await _db.BulkInsertOrUpdateAsync(filteredRoles, bulkConfig);
            await trans.CommitAsync();

            if (bulkConfig.StatsInfo != null)
            {
                // log number of items updated/inserted to the DB
                result.RecordsUpdated = bulkConfig.StatsInfo.StatsNumberUpdated + bulkConfig.StatsInfo.StatsNumberInserted;
            }
        }

        return result;
    }

    public async Task<IEnumerable<RtsOrganisationAndRole>> GetOrganisationsAndRoles(string _lastUpdated)
    {
        var stopwatch = Stopwatch.StartNew();
        int pageSize = 500;
        int maxConcurrency = 10;

        var result = new ConcurrentBag<RtsOrganisationAndRole>();
        var totalRecords = await FetchPageCountAsync(_lastUpdated);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        var pageIndices = Enumerable.Range(0, totalPages);

        await Parallel.ForEachAsync(pageIndices, new ParallelOptions { MaxDegreeOfParallelism = maxConcurrency }, async (page, _) =>
        {
            int offset = page * pageSize;
            var data = await FetchOrganisationAndRolesAsync(_lastUpdated, offset, pageSize);
            foreach (var item in data)
            {
                result.Add(item);
            }
        });

        stopwatch.Stop();

        //var finalResult = result
        //    .DistinctBy(x => new { x.rtsOrganisation.Id, x.rtsRole })
        //    .ToList();

        Console.WriteLine($"Data fetching completed in: {stopwatch.Elapsed.Hours:D2}h:{stopwatch.Elapsed.Minutes:D2}m:{stopwatch.Elapsed.Seconds:D2}s");
        Console.WriteLine($"Total records fetched: {result.Count}");

        return result;
    }


    public async Task<int> FetchPageCountAsync(string _lastUpdated)
    {
        ApiResponse<RtsOrganisationsAndRolesResponse>? result =
            await rtsClient.GetOrganisationsAndRoles(_lastUpdated, 0, 1);

        return result?.Content?.total ?? -1;
    }

    private dynamic ConvertJsonElementToDynamic(JsonElement jsonElement)
    {
        // Using Newtonsoft.Json (Json.NET) to convert
        var jsonString = jsonElement.GetRawText();
        return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonString);
    }

    public RtsOrganisationAndRole TransformOrganisationAndRoles(Entry entry)
    {
        var rtsOrganisationRole = new RtsOrganisationAndRole();
        try
        {
            var rtsOrganisation = new Organisation
            {
                Id = entry.resource.id,
                OId = entry.resource.identifier[0].value,
                Imported = DateTime.Now,
                LastUpdated = entry.resource.meta.lastUpdated,
                SystemUpdated = DateTime.Now,
                Status = entry.resource.active, // Change to boolean in database/
                Address = entry.resource.address[0].text,
                //CountryIdentifier = input.UKCountryIdentifier,
                CountryName = entry.resource.address[0].country,
                Name = entry.resource.name,
                TypeId = entry.resource.type[0].coding[0].code,
                TypeName = entry.resource.type[0].text
            };

            rtsOrganisationRole.rtsOrganisation = rtsOrganisation;
            rtsOrganisationRole.rtsRole = [];
            var extensions = entry.resource.extension;

            for (var i = 0; i < extensions.Count; i++) // Starting from index 2 (third element)
            {
                var extension = ConvertJsonElementToDynamic(extensions[i]);

                if (extension.extension != null)
                {
                    var roleExtensions = extension.extension;
                    DateTime? startdate = null;
                    DateTime? enddate = null;
                    var status = "Active";
                    var identifier = "";
                    var rtsRole = new OrganisationRole();
                    var scoper = -1;
                    for (var j = 0; j < roleExtensions.Count; j++)
                    {
                        var roleExtension = roleExtensions[j];
                        if (roleExtension.url == "startDate")
                        {
                            startdate = roleExtension.valueDate;
                        }

                        if (roleExtension.url == "status")
                        {
                            status = roleExtension.valueString;
                        }

                        if (roleExtension.url == "identifier")
                        {
                            identifier = roleExtension.valueString;
                        }

                        if (roleExtension.url == "endDate")
                        {
                            enddate = roleExtension.valueDate;
                        }

                        if (roleExtension.url == "scoper")
                        {
                            // Split the URL path by '/'

                            string[] segments = roleExtension.valueReference.reference.ToString().Split("/");

                            // Extract the last segment, which is the ID
                            scoper = int.Parse(segments[segments.Length - 1]);
                        }

                        rtsRole = new OrganisationRole
                        {
                            Id = identifier,
                            OrganisationId = entry.resource.identifier[0].value,
                            EndDate = enddate, // Make Nullabale in Database
                            Imported = DateTime.Now,
                            //LastUpdated = input.ModifiedDate, // Get rid of in database.
                            StartDate = startdate,
                            SystemUpdated = DateTime.Now,
                            Scoper = scoper,
                            //CreatedDate = input.CreatedDate.GetValueOrDefault(), // Can't find could be

                            Status = status
                        };
                    }

                    rtsOrganisationRole.rtsRole.Add(rtsRole);
                }
            }
            rtsOrganisationRole.rtsOrganisation.Roles = rtsOrganisationRole.rtsRole;
            return rtsOrganisationRole;
        }
        catch (Exception ex)
        {
            _logger.LogAsInformation($"Error: {ex}");
            return rtsOrganisationRole;
        }
    }

    private async Task<IEnumerable<RtsOrganisationAndRole>> FetchOrganisationAndRolesAsync(string _lastUpdated,
        int _offset, int _count)
    {
        try
        {
            var result = await rtsClient.GetOrganisationsAndRoles(_lastUpdated, _offset, _count);

            var allData = result?.Content?.entry
                ?.Select(x => TransformOrganisationAndRoles(x))
                .ToList();

            return allData ?? Enumerable.Empty<RtsOrganisationAndRole>();
        }
        catch (Exception ex)
        {
            // Optional: Log and rethrow or return empty on failure
            Console.WriteLine($"Error fetching data at offset {_offset}: {ex.Message}");
            return Enumerable.Empty<RtsOrganisationAndRole>();
        }
    }
}