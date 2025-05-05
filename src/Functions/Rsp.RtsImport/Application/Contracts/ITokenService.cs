namespace Rsp.RtsImport.Application.Contracts;

/// <summary>
/// Defines a contract for a service that provides access tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Asynchronously retrieves an access token.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the access token as a string.</returns>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
}