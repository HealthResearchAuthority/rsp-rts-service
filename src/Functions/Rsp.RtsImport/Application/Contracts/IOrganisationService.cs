using Rsp.RtsImport.Application.DTO;
using Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsImport.Application.Contracts;

public interface IOrganisationService
{
    Task<DbOperationResult> UpdateOrganisations(IEnumerable<Organisation> items, bool onlyActive = false);

    Task<DbOperationResult> UpdateRoles(IEnumerable<OrganisationRole> items, bool onlyActive = false);

    Task<IEnumerable<RtsOrganisationAndRole>> GetOrganisationsAndRoles(string lastUpdated);

    Task<int> FetchPageCountAsync(string lastUpdated);

    RtsOrganisationAndRole TransformOrganisationAndRoles(RtsFhirEntry entry);
}