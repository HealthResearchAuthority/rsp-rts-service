using Refit;
using Rsp.RtsImport.Application.DTO.Responses;

namespace Rsp.RtsImport.Application.ServiceClients;

public interface IRtsServiceClient
{
    [Get("/api/v2/Rts/GetOrganisationList")]
    [Headers("Authorization: Bearer")]
    public Task<ApiResponse<RtsResponse>> GetOrganisations([Query] string? modifiedDate, [Query] int pageNumber, [Query] int pageSize);

    [Get("/api/v1/Rts/GetOrganisationRoleList")]
    [Headers("Authorization: Bearer")]
    public Task<ApiResponse<RtsResponse>> GetRoles([Query] string? modifiedDate, [Query] int pageNumber, [Query] int pageSize);

    [Get("/api/v1/Rts/GetTermsetList")]
    [Headers("Authorization: Bearer")]
    public Task<ApiResponse<RtsResponse>> GetTermsets([Query] string? modifiedDate, [Query] int pageNumber, [Query] int pageSize);
}