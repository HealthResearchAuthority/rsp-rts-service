using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Rsp.IrasPortal.Application.ServiceClients;

namespace Rsp.RtsImport.Infrastructure.HttpMessageHandlers;

/// <summary>
/// Delegating handler to add authorization header, before calling external api
/// </summary>
/// <seealso cref="DelegatingHandler" />
public class RtsAuthHeadersHandler(IHttpContextAccessor httpContextAccessor, IRtsAuthorisationServiceClient authClient) : DelegatingHandler
{
    /// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!request.RequestUri.AbsolutePath.Contains("auth"))
        {
            // get authorisation token for the RTS API
            var token = await authClient.GetBearerTokenAsync("grant_type=client_credentials&scope=openid%2Bprofile%2Bemail&client_id=aaU7PY4hz_x_RVOyCo09CulPbTca&client_secret=f2tHDyVGv2FBkZmmAXbryfL7hr0a", cancellationToken);

            if (token?.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(token?.Content?.AccessToken))
            {
                request.Headers.Remove(HeaderNames.Authorization);
                request.Headers.Add(HeaderNames.Authorization, $"Bearer {token.Content.AccessToken}");
            }
        }

        // do not create a private field for HttpContext
        //var context = httpContextAccessor.HttpContext!;

        //context.Items.TryGetValue(ContextItemKeys.AcessToken, out var bearerToken);

        // Use the token to make the call.
        return await base.SendAsync(request, cancellationToken);
    }
}