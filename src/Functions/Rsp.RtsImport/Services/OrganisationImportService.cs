using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsImport.Services;

public class OrganisationImportService : IOrganisationImportService
{
    private readonly ILogger<OrganisationImportService> _logger;
    private readonly IOrganisationService _organisationService;

    public OrganisationImportService(ILogger<OrganisationImportService> logger,
        IOrganisationService organisationService)
    {
        _logger = logger;
        _organisationService = organisationService;
    }

    public async Task<int> ImportOrganisationsAndRoles(string _lastUpdated, bool onlyActive = false)
    {
        _logger.LogAsInformation("Fetching organisation items from API.");
        var resultsOrgAndRoles = await _organisationService.GetOrganisationsAndRoles(_lastUpdated);
        _logger.LogAsInformation(resultsOrgAndRoles.Count().ToString(), "Number of organisation items fetched");
        var test = resultsOrgAndRoles.ToList();
        if (resultsOrgAndRoles != null && resultsOrgAndRoles.Any())
        {
            _logger.LogAsInformation(resultsOrgAndRoles.Count().ToString(), "Number of organisation items saving to database");

            int updatesCounter = 0;
            var resultsOrg = resultsOrgAndRoles.Select(x => x.rtsOrganisation);
            var saveOrg = await _organisationService.UpdateOrganisations(resultsOrg, onlyActive);
            updatesCounter += saveOrg.RecordsUpdated;
            var finalMessageOrg = $"Successfully finished saving organisation items to the database. Updated: {saveOrg.RecordsUpdated} organisation items";
            _logger.LogAsInformation(finalMessageOrg);
            var resultsRoles = resultsOrgAndRoles.Select(x => x.rtsRole);
            IEnumerable<OrganisationRole> resultsRolesflattened = resultsRoles.SelectMany(innerList => innerList);
            _logger.LogAsInformation(resultsOrg.Count().ToString(), "Number of role items saving to the database");

            var saveRole = await _organisationService.UpdateRoles(resultsRolesflattened, onlyActive);
            updatesCounter += saveRole.RecordsUpdated;

            var finalMessageRole = $"Successfully finished saving role items to the database. Updated: {saveRole.RecordsUpdated} role items";
            _logger.LogAsInformation(finalMessageRole);

            var finalMessage = $"Successfully finished saving Organisation and Role items to the database. Updated: {updatesCounter} role items";
            _logger.LogAsInformation(finalMessageRole);
            return updatesCounter;
        }

        return 0;
    }
}