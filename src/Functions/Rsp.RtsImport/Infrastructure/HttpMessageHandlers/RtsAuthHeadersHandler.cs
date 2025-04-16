using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Rsp.RtsImport.Application.ServiceClients;
using Rsp.RtsImport.Application.Settings;

namespace Rsp.RtsImport.Infrastructure.HttpMessageHandlers;

/// <summary>
///     Delegating handler to add authorization header, before calling external api
/// </summary>
/// <seealso cref="DelegatingHandler" />
public class RtsAuthHeadersHandler
(
    IRtsAuthorisationServiceClient authClient,
    IOptions<AppSettings> appSettingsOptions
) : DelegatingHandler
{
    private readonly AppSettings _appSettings = appSettingsOptions.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request?.RequestUri?.AbsolutePath?.Contains("auth") != true)
        {
            // Build the request body from app settings
            var requestBody = "grant_type=client_credentials&=openid%2Bprofile%2Bemail" +
                              $"&client_id={Uri.EscapeDataString(_appSettings.RtsApiClientId ?? string.Empty)}" +
                              $"&client_secret={Uri.EscapeDataString(_appSettings.RtsApiClientSecret ?? string.Empty)}";

            var token = await authClient.GetBearerTokenAsync(requestBody, cancellationToken);

            if (token?.StatusCode == HttpStatusCode.OK &&
                !string.IsNullOrEmpty(token.Content?.AccessToken))
            {
                request?.Headers.Remove(HeaderNames.Authorization);
                request?.Headers.Add(HeaderNames.Authorization, $"Bearer {token.Content.AccessToken}");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}