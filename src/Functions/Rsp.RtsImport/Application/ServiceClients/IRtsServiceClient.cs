using Refit;
using Rsp.RtsImport.Application.DTO.Responses;

namespace Rsp.RtsImport.Application.ServiceClients;

public interface IRtsServiceClient
{
    [Get("/organization")]
    [Headers("Authorization: Bearer")]
    public Task<ApiResponse<RtsOrganisationsAndRolesResponse>> GetOrganisationsAndRoles
    (
        [AliasAs("_lastUpdated")][Query] string? lastUpdated,
        [AliasAs("_offset")][Query] int offset,
        [AliasAs("_count")][Query] int count
    );
}