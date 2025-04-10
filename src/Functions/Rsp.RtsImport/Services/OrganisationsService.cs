﻿namespace Rsp.RtsImport.Services;

public class OrganisationsService(IRtsServiceClient rtsClient, RtsDbContext db, ILogger<OrganisationsService> logger)
    : IOrganisationService
{
    private readonly RtsDbContext _db = db;
    private readonly ILogger<OrganisationsService> _logger;
    private readonly IRtsServiceClient _rtsClient = rtsClient;

    public async Task<DbOperationResult> UpdateOrganisations(IEnumerable<Organisation> dbRecords,
        bool onlyActive = false)
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
                ConflictOption = ConflictOption.None,
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
                result.RecordsUpdated =
                    bulkConfig.StatsInfo.StatsNumberUpdated + bulkConfig.StatsInfo.StatsNumberInserted;
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
            // Use HashSet for O(1) lookup
            var organisationIds = new HashSet<string>(
                await _db.Organisation.Select(x => x.Id).ToListAsync()
            );

            // This is now super fast (O(n))
            var filteredRoles = dbRecords
                .Where(x => organisationIds.Contains(x.OrganisationId))
                .ToList(); // Materialize only once for BulkInsertOrUpdateAsync

            if (!filteredRoles.Any())
            {
                result.RecordsUpdated = 0;
                return result;
            }

            _db.Database.SetCommandTimeout(500);

            var bulkConfig = new BulkConfig
            {
                UseTempDB = true,
                SetOutputIdentity = false,
                ConflictOption = ConflictOption.Replace, // Ensure both insert and update
                BatchSize = 10000,
                CalculateStats = true,
                BulkCopyTimeout = 300
            };

            await _db.BulkInsertOrUpdateAsync(filteredRoles, bulkConfig);
            await trans.CommitAsync();

            if (bulkConfig.StatsInfo != null)
            {
                result.RecordsUpdated =
                    bulkConfig.StatsInfo.StatsNumberUpdated + bulkConfig.StatsInfo.StatsNumberInserted;
            }
        }

        return result;
    }


    public async Task<IEnumerable<RtsOrganisationAndRole>> GetOrganisationsAndRoles(string _lastUpdated)
    {
        var pageSize = 500;
        var maxConcurrency = 10;

        var result = new ConcurrentBag<RtsOrganisationAndRole>();
        var totalRecords = await FetchPageCountAsync(_lastUpdated);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        var pageIndices = Enumerable.Range(0, totalPages);

        await Parallel.ForEachAsync(pageIndices, new ParallelOptions { MaxDegreeOfParallelism = maxConcurrency },
            async (page, _) =>
            {
                var offset = page * pageSize;
                var data = await FetchOrganisationAndRolesAsync(_lastUpdated, offset, pageSize);
                foreach (var item in data)
                {
                    result.Add(item);
                }
            });

        var finalResult = result
            .DistinctBy(x => new { x.rtsOrganisation.Id, x.rtsRole })
            .ToList();

        return finalResult;
    }


    public async Task<int> FetchPageCountAsync(string _lastUpdated)
    {
        ApiResponse<RtsOrganisationsAndRolesResponse>? result =
            await rtsClient.GetOrganisationsAndRoles(_lastUpdated, 0, 1);

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

            rtsOrganisationRole.rtsOrganisation = rtsOrganisation;
            rtsOrganisationRole.rtsRole = [];
            var extensions = entry.Resource.Extension;

            foreach (var extension in extensions)
            {
                if (extension.Extension is { Count: > 0 })
                {
                    var roleExtensions = extension.Extension;
                    DateTime? startdate = null;
                    DateTime? enddate = null;
                    var status = "Active";
                    var identifier = "";
                    var rtsRole = new OrganisationRole();
                    var scoper = -1;

                    foreach (var roleExtension in roleExtensions)
                    {
                        switch (roleExtension.Url)
                        {
                            case "startDate":
                                startdate = Convert.ToDateTime(roleExtension.ValueDate);
                                break;
                            case "status":
                                status = roleExtension.ValueString;
                                break;
                            case "identifier":
                                identifier = roleExtension.ValueString;
                                break;
                            case "endDate":
                                enddate = Convert.ToDateTime(roleExtension.ValueDate);
                                break;
                            case "scoper":
                            {
                                // Split the URL path by '/'

                                var segments = roleExtension.ValueReference.Reference.Split("/");

                                // Extract the last segment, which is the ID
                                scoper = int.Parse(segments[segments.Length - 1]);
                                break;
                            }
                        }
                    }

                    rtsRole = new OrganisationRole
                    {
                        Id = identifier,
                        OrganisationId = entry.Resource.Id,
                        Imported = DateTime.Now,
                        StartDate = startdate,
                        SystemUpdated = DateTime.Now,
                        Scoper = scoper,
                        CreatedDate = DateTime.Now, // Can't find could be
                        Status = status
                    };

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

            var allData = result?.Content?.Entry
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