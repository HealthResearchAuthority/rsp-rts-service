using Rsp.RtsImport.Application.DTO;
using Rsp.RtsImport.Application.DTO.Responses;

namespace Rsp.RtsImport.Application.Contracts;

public interface IOrganisationService
{
    Task<DbOperationResult> UpdateOrganisations(IEnumerable<RtsOrganisation> items, bool onlyActive = false);

    Task<DbOperationResult> UpdateRoles(IEnumerable<RtsRole> items, bool onlyActive = false);

    Task<DbOperationResult> UpdateTermsets(IEnumerable<RtsTermset> items);

    Task<IEnumerable<RtsTermset>> GetTermsets(string modifiedDate);

    Task<IEnumerable<RtsOrganisation>> GetOrganisations(string modifiedDate);

    Task<IEnumerable<RtsRole>> GetRoles(string modifiedDate);

    Task<int> FetchPageCountAsync(string modifiedDate, string dataType);
}