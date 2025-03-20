using System.Data;
using System.Text.Json;
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
        //Step 1: initiate semaphore to manage concurency
        var semaphore = new SemaphoreSlim(3);

        // initiate an empty array to hold results
        var result = new List<RtsOrganisationAndRole>();

        // Step 2: Fetch data using the access token
        // setup pagination
        int _count = 500;

        // get the number of pages needed for the request
        var totalRecords = await FetchPageCountAsync(_lastUpdated);
        var totalPages = totalRecords / _count + 1;

        var tasks = new List<Task<IEnumerable<RtsOrganisationAndRole>>>();

        for (int interval = 1; interval <= totalPages; interval++)
        {
            await semaphore.WaitAsync();
            var _offset = interval * _count;
            // collect all fetching tasks
            tasks.Add(FetchOrganisationAndRolesAsync(_lastUpdated, _offset, _count, semaphore));
        }
        // execute all fetching tasks in paralel
        var allData = await Task.WhenAll(tasks);

        foreach (var task in allData)
        {
            result.AddRange(task);
        }

        // cleanup duplicates if any
        var uniqueResultList = result.DistinctBy(x => x.rtsOrganisation.Id); // TODO: Need to make sure it distinct for both

        return uniqueResultList;
    }

    public async Task<int> FetchPageCountAsync(string _lastUpdated)
    {
        ApiResponse<RtsOrganisationsAndRolesResponse>? result = await _rtsClient.GetOrganisationsAndRoles(_lastUpdated, 1, 1);

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

            for (int i = 0; i < extensions.Count; i++) // Starting from index 2 (third element)
            {
                var extension = ConvertJsonElementToDynamic(extensions[i]);

                if (extension.extension != null)
                {
                    var roleExtensions = extension.extension;
                    DateTime? startdate = null;
                    DateTime? enddate = null;
                    string status = "Active";
                    string identifier = "";
                    OrganisationRole rtsRole = new OrganisationRole();
                    int scoper = -1;
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
                            EndDate = enddate,// Make Nullabale in Database
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

    private async Task<IEnumerable<RtsOrganisationAndRole>> FetchOrganisationAndRolesAsync(string _lastUpdated, int _offset, int _count, SemaphoreSlim semaphore)
    {
        try
        {
            var result = await _rtsClient.GetOrganisationsAndRoles(_lastUpdated, _offset, _count);
            var allData = result?.Content?.entry
                                .Select(x => TransformOrganisationAndRoles(x))
                                .ToList();  // Ensure the selection is evaluated to a list immediately

            return allData ?? Enumerable.Empty<RtsOrganisationAndRole>();  // Return an empty collection if the result is null
        }
        finally
        {
            semaphore.Release();
        }
    }
}