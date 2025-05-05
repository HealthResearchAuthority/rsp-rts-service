using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Refit;
using Rsp.RtsImport.Application.DTO.Responses;
using Rsp.RtsImport.Application.ServiceClients;
using Rsp.RtsImport.Services;
using Shouldly;

namespace Rts.RtsImport.UnitTests.Services;

public class TokenServiceTests : TestServiceBase
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact]
    public async Task GetAccessTokenAsync_ShouldReturnToken_FromCache()
    {
        // Arrange
        const string expectedToken = "cached-token";

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        memoryCache.Set("NIHR_BearerToken", expectedToken);

        Mocker.Use<IMemoryCache>(memoryCache);

        var tokenService = Mocker.CreateInstance<TokenService>();

        // Act
        var token = await tokenService.GetAccessTokenAsync(_cancellationToken);

        // Assert
        token.ShouldBe(expectedToken);
    }

    [Fact]
    public async Task GetAccessTokenAsync_ShouldFetchToken_AndCacheIt_WhenNotInCache()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        const string expectedToken = "new-token";
        const int expiresIn = 3600;

        var tokenResponse = new RtsAuthResponse
        {
            AccessToken = expectedToken,
            ExpiresIn = expiresIn
        };

        Mocker
            .GetMock<IRtsAuthorisationServiceClient>()
            .Setup(c => c.GetBearerTokenAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(new ApiResponse<RtsAuthResponse?>
            (
                new HttpResponseMessage(HttpStatusCode.OK),
                tokenResponse,
                new()
            ));

        Mocker.Use<IMemoryCache>(memoryCache);

        var tokenService = Mocker.CreateInstance<TokenService>();

        // Act
        var token = await tokenService.GetAccessTokenAsync(_cancellationToken);

        // Assert
        token.ShouldBe(expectedToken);

        memoryCache.TryGetValue("NIHR_BearerToken", out string? cachedToken).ShouldBeTrue();
        cachedToken.ShouldBe(expectedToken);
    }

    [Fact]
    public async Task GetAccessTokenAsync_ShouldReturnEmptyString_WhenTokenRetrievalFails()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        Mocker
            .GetMock<IRtsAuthorisationServiceClient>()
            .Setup(c => c.GetBearerTokenAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(new ApiResponse<RtsAuthResponse?>
            (
                new HttpResponseMessage(HttpStatusCode.InternalServerError),
                null,
                new()
            ));

        Mocker.Use<IMemoryCache>(memoryCache);

        var tokenService = Mocker.CreateInstance<TokenService>();

        // Act
        var token = await tokenService.GetAccessTokenAsync(_cancellationToken);

        // Assert
        token.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("")]
    public async Task GetAccessTokenAsync_ShouldReturnEmptyString_WhenAccessTokenIsNullOrWhitespace(string? accessToken)
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        Mocker
            .GetMock<IRtsAuthorisationServiceClient>()
            .Setup(c => c.GetBearerTokenAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(new ApiResponse<RtsAuthResponse?>
            (
                new HttpResponseMessage(HttpStatusCode.InternalServerError),
                new RtsAuthResponse
                {
                    AccessToken = accessToken!,
                    ExpiresIn = 3600
                },
                new()
            ));

        Mocker.Use<IMemoryCache>(memoryCache);

        var tokenService = Mocker.CreateInstance<TokenService>();

        // Act
        var token = await tokenService.GetAccessTokenAsync(_cancellationToken);

        // Assert
        token.ShouldBeEmpty();
    }
}