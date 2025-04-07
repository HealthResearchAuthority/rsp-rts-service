namespace Rsp.RtsImport.Application.ServiceClients;

public interface IRtsServiceClient
{
    [Get("/api/organization")]
    [Headers("Authorization: Bearer")]
    public Task<ApiResponse<RtsOrganisationsAndRolesResponse>> GetOrganisationsAndRoles([Query] string? _lastUpdated, [Query] int _offset, [Query] int _count);
}