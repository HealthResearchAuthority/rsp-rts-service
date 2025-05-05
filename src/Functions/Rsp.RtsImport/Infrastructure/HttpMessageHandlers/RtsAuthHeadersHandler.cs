using Microsoft.Net.Http.Headers;
using Rsp.RtsImport.Application.Contracts;

namespace Rsp.RtsImport.Infrastructure.HttpMessageHandlers;

/// <summary>
/// A delegating handler that adds an authorization header to outgoing HTTP requests.
/// This handler retrieves an access token from the provided <see cref="ITokenService"/>
/// and attaches it to the request headers, unless the request is for an "auth" endpoint.
/// </summary>
/// <seealso cref="DelegatingHandler" />
public class RtsAuthHeadersHandler(ITokenService tokenService) : DelegatingHandler
{
    /// <summary>
    /// Sends an HTTP request with an authorization header if applicable.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Retrieve the access token from the token service
        var token = await tokenService.GetAccessTokenAsync(cancellationToken);

        // If a valid token is retrieved, add it to the Authorization header
        if (!string.IsNullOrEmpty(token))
        {
            request?.Headers.Remove(HeaderNames.Authorization);
            request?.Headers.Add(HeaderNames.Authorization, $"Bearer {token}");
        }

        // Proceed with the request by calling the base handler
        return await base.SendAsync(request, cancellationToken);
    }
}