using Rsp.RtsImport.Application.Contracts;

namespace Rsp.RtsImport.Services;

public class OrganisationImportService(
    IOrganisationService organisationService,
    IAuditService auditService
) : IOrganisationImportService
{
    public async Task<int> ImportOrganisationsAndRoles(string lastUpdated, bool onlyActive = false)
    {
        await auditService.ApiDownloadStarted();

        var resultsOrgAndRoles = await organisationService.GetOrganisationsAndRoles(lastUpdated);

        await auditService.ApiDownloadCompleted(resultsOrgAndRoles.Count());

        if (resultsOrgAndRoles != null && resultsOrgAndRoles.Any())
        {
            await auditService.DatabaseOrganisationInsertStarted();

            var updatesCounter = 0;
            var resultsOrg = resultsOrgAndRoles.Select(x => x.RtsOrganisation);

            await auditService.DatabaseOrganisationInsertStarted();

            var saveOrg = await organisationService.UpdateOrganisations(resultsOrg, onlyActive);

            await auditService.DatabaseOrganisationInsertCompleted(saveOrg.RecordsUpdated);

            updatesCounter += saveOrg.RecordsUpdated;

            var resultsRoles = resultsOrgAndRoles.Select(x => x.RtsRole);
            var resultsRolesflattened = resultsRoles.SelectMany(innerList => innerList);

            await auditService.DatabaseOrganisationRolesInsertStarted();

            var saveRole = await organisationService.UpdateRoles(resultsRolesflattened, onlyActive);
            updatesCounter += saveRole.RecordsUpdated;

            await auditService.DatabaseOrganisationRolesInsertCompleted(saveRole.RecordsUpdated);

            await auditService.DatabaseSponsorOrganisationUpdateStarted();

            var sponsorOrganisationsCount = await organisationService.UpdateSponsorOrganisations(resultsOrg);

            await auditService.DatabaseSponsorOrganisationUpdateCompleted(sponsorOrganisationsCount);

            return updatesCounter;
        }

        return 0;
    }
}