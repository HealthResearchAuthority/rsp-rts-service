namespace Rsp.RtsImport.Application.ServiceClients;

public interface IRtsAuthorisationServiceClient
{
    [Post("/oauth2/token")]
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    public Task<ApiResponse<RtsAuthResponse?>> GetBearerTokenAsync([Body] string body,
        CancellationToken cancellationToken);
}