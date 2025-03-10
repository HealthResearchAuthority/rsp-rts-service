using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Rsp.RtsImport.Application.Settings;

namespace Rsp.RtsImport.Infrastructure.HttpMessageHandlers;

/// <summary>
/// Delegating handler to add authorization header, before calling external api
/// </summary>
/// <seealso cref="DelegatingHandler" />
public class RtsPreAuthHeadersHandler(IHttpContextAccessor httpContextAccessor, AppSettings config) : DelegatingHandler
{
    /// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // get authorisation token for the RTS API

        request.Headers.Remove(HeaderNames.Host);
        request.Headers.Remove("Authority");
        request.Headers.Remove(HeaderNames.Authorization);

        request.Headers.Add(HeaderNames.Host, config.RtsApiBaseUrl.Authority);
        request.Headers.Add("Authority", config.RtsApiBaseUrl.Authority);
        request.Headers.Add(HeaderNames.Authorization, $"Basic {config.RtsApiSecret}");

        // Use the token to make the call.
        return await base.SendAsync(request, cancellationToken);
    }
}