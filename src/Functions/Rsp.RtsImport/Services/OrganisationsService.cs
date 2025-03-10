using System.Data;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Refit;
using Rsp.RtsImport.Application;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Application.DTO;
using Rsp.RtsImport.Application.DTO.Responses;
using Rsp.RtsImport.Application.ServiceClients;
using Rsp.RtsService.Infrastructure;

namespace Rsp.RtsImport.Services;

public class OrganisationsService(IRtsServiceClient rtsClient, RtsDbContext db) : IOrganisationService
{
    private readonly IRtsServiceClient _rtsClient = rtsClient;
    private readonly RtsDbContext _db = db;

    public async Task<DbOperationResult> UpdateOrganisations(IEnumerable<RtsOrganisation> items, bool onlyActive = false)
    {
        var result = new DbOperationResult();

        // convert models to DB entities
        var dbRecords = items.Select(x => x.ConvertToDbModel());

        if (onlyActive)
        {
            dbRecords = dbRecords.Where(x => x.Status == RtsRecordStatusOptions.Active);
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

    public async Task<DbOperationResult> UpdateTermsets(IEnumerable<RtsTermset> items)
    {
        // convert models to DB entities
        var dbRecords = items.Select(x => x.ConvertToDbModel());

        var result = new DbOperationResult();
        using (var trans = await _db.Database.BeginTransactionAsync())
        {
            _db.Database.SetCommandTimeout(500);
            var bulkConfig = new BulkConfig
            {
                UseTempDB = true,
                SetOutputIdentity = false,
                ConflictOption = EFCore.BulkExtensions.ConflictOption.None,
                BatchSize = 10000,
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

    public async Task<DbOperationResult> UpdateRoles(IEnumerable<RtsRole> items, bool onlyActive = false)
    {
        // convert models to DB entities
        var dbRecords = items.Select(x => x.ConvertToDbModel());
        if (onlyActive)
        {
            dbRecords = dbRecords.Where(x => x.Status == RtsRecordStatusOptions.Active);
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

    public async Task<IEnumerable<RtsTermset>> GetTermsets(string modifiedDate)
    {
        //Step 1: initiate semaphore to manage concurency
        var semaphore = new SemaphoreSlim(3);

        // initiate an empty array to hold results
        var result = new List<RtsTermset>();

        // Step 2: Fetch data using the access token
        // setup pagination
        int pageSize = 10000;

        // get the number of pages needed for the request
        var totalRecords = await FetchPageCountAsync(modifiedDate, "termset");
        var totalPages = totalRecords / pageSize + 1;
        var tasks = new List<Task<IEnumerable<RtsTermset>>>();

        for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
        {
            await semaphore.WaitAsync();

            // collect all fetching tasks
            tasks.Add(FetchTermsetsAsync(modifiedDate, pageNumber, pageSize, semaphore));
        }
        // execute all fetching tasks in paralel
        var allData = await Task.WhenAll(tasks);
        foreach (var task in allData)
        {
            result.AddRange(task);
        }

        // cleanup duplicates if any
        var uniqueResultList = result.DistinctBy(x => x.Identifier);

        return uniqueResultList;
    }

    public async Task<IEnumerable<RtsRole>> GetRoles(string modifiedDate)
    {
        //Step 1: initiate semaphore to manage concurency
        var semaphore = new SemaphoreSlim(3);

        // initiate an empty array to hold results
        var result = new List<RtsRole>();

        // Step 2: Fetch data using the access token
        // setup pagination
        int pageSize = 10000;

        // get the number of pages needed for the request
        var totalRecords = await FetchPageCountAsync(modifiedDate, "role");
        var totalPages = totalRecords / pageSize + 1;
        var tasks = new List<Task<IEnumerable<RtsRole>>>();

        for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
        {
            await semaphore.WaitAsync();

            // collect all fetching tasks
            tasks.Add(FetchRolesAsync(modifiedDate, pageNumber, pageSize, semaphore));
        }
        // execute all fetching tasks in paralel
        var allData = await Task.WhenAll(tasks);
        foreach (var task in allData.Where(t => t != null))
        {
            result.AddRange(task);
        }

        // cleanup duplicates if any
        var uniqueResultList = result.DistinctBy(x => (x.RoleType, x.OrgIdentifier, x.ParentIdentifier, x.CreatedDate));

        return uniqueResultList;
    }

    public async Task<IEnumerable<RtsOrganisation>> GetOrganisations(string modifiedDate)
    {
        //Step 1: initiate semaphore to manage concurency
        var semaphore = new SemaphoreSlim(3);

        // initiate an empty array to hold results
        var result = new List<RtsOrganisation>();

        // Step 2: Fetch data using the access token
        // setup pagination
        int pageSize = 10000;

        // get the number of pages needed for the request
        var totalRecords = await FetchPageCountAsync(modifiedDate, "organisation");
        var totalPages = totalRecords / pageSize + 1;
        var tasks = new List<Task<IEnumerable<RtsOrganisation>>>();

        for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
        {
            await semaphore.WaitAsync();

            // collect all fetching tasks
            tasks.Add(FetchOrganisationsAsync(modifiedDate, pageNumber, pageSize, semaphore));
        }
        // execute all fetching tasks in paralel
        var allData = await Task.WhenAll(tasks);
        foreach (var task in allData)
        {
            result.AddRange(task);
        }

        // cleanup duplicates if any
        var uniqueResultList = result.DistinctBy(x => x.Identifier);

        return uniqueResultList;
    }

    public async Task<int> FetchPageCountAsync(string modifiedDate, string dataType)
    {
        ApiResponse<RtsResponse>? result = null;

        switch (dataType)
        {
            case "organisation":
                result = await _rtsClient.GetOrganisations(modifiedDate, 1, 1);
                break;

            case "role":
                result = await _rtsClient.GetRoles(modifiedDate, 1, 1);
                break;

            case "termset":
                result = await _rtsClient.GetTermsets(modifiedDate, 1, 1);
                break;
        }

        return result?.Content?.Result?.TotalRecords ?? -1;
    }

    private async Task<IEnumerable<RtsTermset>> FetchTermsetsAsync(string modifiedDate, int pageNumber, int pageSize, SemaphoreSlim semaphore)
    {
        try
        {
            var result = await _rtsClient.GetTermsets(modifiedDate, pageNumber, pageSize);
            return result?.Content?.Result?.RtsTermsets;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<IEnumerable<RtsRole>> FetchRolesAsync(string modifiedDate, int pageNumber, int pageSize, SemaphoreSlim semaphore)
    {
        try
        {
            var result = await _rtsClient.GetRoles(modifiedDate, pageNumber, pageSize);
            return result?.Content?.Result?.RtsOrganisationRoles;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task<IEnumerable<RtsOrganisation>> FetchOrganisationsAsync(string modifiedDate, int pageNumber, int pageSize, SemaphoreSlim semaphore)
    {
        try
        {
            var result = await _rtsClient.GetOrganisations(modifiedDate, pageNumber, pageSize);
            return result?.Content?.Result?.RtsOrganisations;
        }
        finally
        {
            semaphore.Release();
        }
    }
}