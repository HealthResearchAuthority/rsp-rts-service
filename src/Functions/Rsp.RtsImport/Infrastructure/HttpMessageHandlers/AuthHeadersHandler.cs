using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Rsp.MalwareScanEvent.Application.Configuration;
using Rsp.RtsImport.Application.Settings;
using Rsp.RtsImport.Functions;

namespace Rsp.RtsImport.Infrastructure.HttpMessageHandlers;

/// <summary>
/// Delegating handler to add authorization header, before calling external api
/// </summary>
/// <seealso cref="DelegatingHandler"/>
public class AuthHeadersHandler(TokenCredential credential, AppSettings appSettings) : DelegatingHandler
{
    /// <summary>
    /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="request"/> was <see langword="null"/>.
    /// </exception>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //var accessToken = await credential.GetTokenAsync(
        //        new TokenRequestContext([appSettings.MicrosoftEntra.Audience]), cancellationToken);
        //request.Headers.Remove(HeaderNames.Authorization);
        //request.Headers.Add(HeaderNames.Authorization, $"Bearer {accessToken.Token}");
        //Console.WriteLine(accessToken.Token);

        // Use the token to make the call.
        return await base.SendAsync(request, cancellationToken);
    }
}