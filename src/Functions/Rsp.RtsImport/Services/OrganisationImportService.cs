using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.RtsImport.Application.Contracts;

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

    public async Task<int> ImportOrganisations(string dateModified, bool onlyActive = false)
    {
        _logger.LogAsInformation("Fetching organisation items from API.");
        var resultsOrg = await _organisationService.GetOrganisations(dateModified);
        _logger.LogAsInformation(resultsOrg.Count().ToString(), "Number of organisation items fetched");

        if (resultsOrg != null && resultsOrg.Any())
        {
            _logger.LogAsInformation(resultsOrg.Count().ToString(), "Number of organisation items saving to database");

            int updatesCounter = 0;
            var save = await _organisationService.UpdateOrganisations(resultsOrg, onlyActive);
            updatesCounter = save.RecordsUpdated;
            var finalMessage = $"Sucesfully finished saving organisation items to the database. Updated: {updatesCounter} organisation items";
            _logger.LogAsInformation(finalMessage);
            return updatesCounter;
        }
        return 0;
    }

    public async Task<int> ImportTermsets(string dateModified)
    {
        _logger.LogAsInformation("Fetching termset items from API.");
        var resultsOrg = await _organisationService.GetTermsets(dateModified);
        _logger.LogAsInformation(resultsOrg.Count().ToString(), "Number of termset items fetched");

        if (resultsOrg != null && resultsOrg.Any())
        {
            _logger.LogAsInformation(resultsOrg.Count().ToString(), "Number of termset items saving to the database.");

            int updatesCounter = 0;
            var save = await _organisationService.UpdateTermsets(resultsOrg);
            updatesCounter = save.RecordsUpdated;

            var finalMessage = $"Sucesfully finished saving termset items to the database. Updated: {updatesCounter} termset items";
            _logger.LogAsInformation(finalMessage);
            return updatesCounter;
        }
        return 0;
    }

    public async Task<int> ImportRoles(string dateModified, bool onlyActive = false)
    {
        _logger.LogAsInformation("Fetching role items from API.");
        var resultsOrg = await _organisationService.GetRoles(dateModified);
        _logger.LogAsInformation(resultsOrg.Count().ToString(), "Number of role items fetched");

        if (resultsOrg != null && resultsOrg.Any())
        {
            _logger.LogAsInformation(resultsOrg.Count().ToString(), "Number of role items saving to the database");

            int updatesCounter = 0;
            var save = await _organisationService.UpdateRoles(resultsOrg, onlyActive);
            updatesCounter = save.RecordsUpdated;

            var finalMessage = $"Sucesfully finished saving role items to the database. Updated: {updatesCounter} role items";
            _logger.LogAsInformation(finalMessage);
            return updatesCounter;
        }
        return 0;
    }
}