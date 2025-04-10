using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsImport.Services;

public class OrganisationImportService
(
    ILogger<OrganisationImportService> logger,
    IOrganisationService organisationService
) : IOrganisationImportService
{
    public async Task<int> ImportOrganisationsAndRoles(string _lastUpdated, bool onlyActive = false)
    {
        logger.LogAsInformation("Fetching organisation items from API.");
        var resultsOrgAndRoles = await organisationService.GetOrganisationsAndRoles(_lastUpdated);
        logger.LogAsInformation(resultsOrgAndRoles.Count().ToString(), "Number of organisation items fetched");

        if (resultsOrgAndRoles != null && resultsOrgAndRoles.Any())
        {
            logger.LogAsInformation(resultsOrgAndRoles.Count().ToString(),
                "Number of organisation items saving to database");

            var updatesCounter = 0;
            var resultsOrg = resultsOrgAndRoles.Select(x => x.RtsOrganisation);
            var saveOrg = await organisationService.UpdateOrganisations(resultsOrg, onlyActive);
            updatesCounter += saveOrg.RecordsUpdated;
            var finalMessageOrg =
                $"Successfully finished saving organisation items to the database. Updated: {saveOrg.RecordsUpdated} organisation items";
            logger.LogAsInformation(finalMessageOrg);
            var resultsRoles = resultsOrgAndRoles.Select(x => x.RtsRole);
            IEnumerable<OrganisationRole> resultsRolesflattened = resultsRoles.SelectMany(innerList => innerList);
            logger.LogAsInformation(resultsOrg.Count().ToString(), "Number of role items saving to the database");

            var saveRole = await organisationService.UpdateRoles(resultsRolesflattened, onlyActive);
            updatesCounter += saveRole.RecordsUpdated;

            var finalMessageRole =
                $"Successfully finished saving role items to the database. Updated: {saveRole.RecordsUpdated} role items";
            logger.LogAsInformation(finalMessageRole);

            var finalMessage =
                $"Successfully finished saving Organisation and Role items to the database. Updated: {updatesCounter} role items";
            logger.LogAsInformation(finalMessage);
            return updatesCounter;
        }

        return 0;
    }
}