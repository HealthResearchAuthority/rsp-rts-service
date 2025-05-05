using Azure.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Application.ServiceClients;
using AppSettings = Rsp.RtsImport.Application.Settings.AppSettings;

namespace Rsp.RtsImport.Services;

/// <summary>
/// Service responsible for managing and retrieving access tokens for authentication.
/// </summary>
public class TokenService
(
    IMemoryCache memoryCache, // Cache for storing the access token temporarily
    IRtsAuthorisationServiceClient authClient, // Client for interacting with the authorization service
    AppSettings appSettings, // Application settings containing client credentials
    ILogger<TokenService> logger // Logger for logging errors and information
) : ITokenService
{
    private const string TokenCacheKey = "NIHR_BearerToken"; // Key used to store the token in the cache

    /// <summary>
    /// Retrieves an access token, either from the cache or by requesting a new one from the authorization service.
    /// </summary>
    /// <returns>A valid access token as a string.</returns>
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        // Check if the token is already cached
        if (memoryCache.TryGetValue(TokenCacheKey, out string? token))
        {
            return token!; // Return the cached token
        }

        // Build the request body using client credentials from app settings
        var credentials = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", Uri.EscapeDataString(appSettings.RtsApiClientId ?? string.Empty) },
            { "client_secret", Uri.EscapeDataString(appSettings.RtsApiClientSecret ?? string.Empty) },
            { "scope", "openid profile email" }
        };

        // Encode the credentials into a form-urlencoded content
        var encodedContent = new FormUrlEncodedContent(credentials);

        // Read the encoded content as a string
        var requestBody = await encodedContent.ReadAsStringAsync();

        // Request a new token from the authorization service
        var tokenResponse = await authClient.GetBearerTokenAsync(requestBody, cancellationToken);

        // Check if the token retrieval was successful
        if (!tokenResponse.IsSuccessStatusCode || string.IsNullOrWhiteSpace(tokenResponse.Content?.AccessToken))
        {
            // Log the error details
            var parameters = $"Status code: {tokenResponse.StatusCode}, Error: {tokenResponse.Error?.Message}";
            logger.LogAsError(parameters, "ERR_TOKEN_RETRIEVAL_FAILED", "Failed to retrieve access token", tokenResponse.Error);

            return string.Empty; // Return an empty string if token retrieval failed
        }

        // Calculate the cache duration (token lifetime minus a buffer of 5 minutes)
        var expiresIn = tokenResponse.Content.ExpiresIn;
        var cacheDuration = TimeSpan.FromSeconds(expiresIn - 300);

        // Cache the token for the calculated duration
        memoryCache.Set(TokenCacheKey, tokenResponse.Content.AccessToken, cacheDuration);

        return tokenResponse.Content.AccessToken; // Return the retrieved token
    }
}