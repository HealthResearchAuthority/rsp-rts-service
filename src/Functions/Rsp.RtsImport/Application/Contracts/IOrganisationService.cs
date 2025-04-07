namespace Rsp.RtsImport.Application.Contracts;

public interface IOrganisationService
{
    Task<DbOperationResult> UpdateOrganisations(IEnumerable<Organisation> items, bool onlyActive = false);

    Task<DbOperationResult> UpdateRoles(IEnumerable<OrganisationRole> items, bool onlyActive = false);

    Task<IEnumerable<RtsOrganisationAndRole>> GetOrganisationsAndRoles(string _lastUpdated);

    Task<int> FetchPageCountAsync(string _lastUpdated);

    RtsOrganisationAndRole TransformOrganisationAndRoles(RtsFhirEntry entry);
}