using System.Net;
using Azure.Core;
using Microsoft.Net.Http.Headers;
using Moq;
using Rsp.MalwareScanEvent.Application.Configuration;
using Rsp.RtsImport.Application.Settings;
using Rsp.RtsImport.Infrastructure.HttpMessageHandlers;
using Shouldly;

// <- adjust namespace to where AuthHeadersHandler lives

namespace Rts.RtsImport.UnitTests.Infrastructure.HttpMessageHandlers;

public class AuthHeadersHandlerTests
{
    [Fact]
    public async Task SendAsync_AddsBearerTokenAuthorizationHeader_AndRemovesExisting()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            MicrosoftEntra = new MicrosoftEntra
            {
                Audience = "api://test-audience"
            }
        };

        var token = new AccessToken("test-token-123", DateTimeOffset.UtcNow.AddMinutes(5));

        var credential = new Mock<TokenCredential>(MockBehavior.Strict);
        credential
            .Setup(c => c.GetTokenAsync(
                It.Is<TokenRequestContext>(ctx =>
                    ctx.Scopes.Length == 1 &&
                    ctx.Scopes[0] == appSettings.MicrosoftEntra.Audience),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var inner = new CaptureRequestHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));

        var sut = new AuthHeadersHandler(credential.Object, appSettings)
        {
            InnerHandler = inner
        };

        using var invoker = new HttpMessageInvoker(sut);

        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/foo");
        request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, "Bearer old-token");

        // Act
        var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        inner.Captured.ShouldNotBeNull();
        inner.Captured!.Headers.Contains(HeaderNames.Authorization).ShouldBeTrue();

        // Should be replaced with new bearer token
        inner.Captured.Headers.GetValues(HeaderNames.Authorization).Single().ShouldBe("Bearer test-token-123");

        credential.VerifyAll();
    }

    [Fact]
    public async Task SendAsync_WhenNoExistingAuthorizationHeader_AddsBearerTokenAuthorizationHeader()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            MicrosoftEntra = new MicrosoftEntra
            {
                Audience = "api://test-audience"
            }
        };

        var token = new AccessToken("abc", DateTimeOffset.UtcNow.AddMinutes(5));

        var credential = new Mock<TokenCredential>(MockBehavior.Strict);
        credential
            .Setup(c => c.GetTokenAsync(
                It.Is<TokenRequestContext>(ctx => ctx.Scopes.Single() == appSettings.MicrosoftEntra.Audience),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var inner = new CaptureRequestHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));

        var sut = new AuthHeadersHandler(credential.Object, appSettings)
        {
            InnerHandler = inner
        };

        using var invoker = new HttpMessageInvoker(sut);

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.test/bar");

        // Act
        var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        inner.Captured.ShouldNotBeNull();
        inner.Captured!.Headers.GetValues(HeaderNames.Authorization).Single().ShouldBe("Bearer abc");

        credential.VerifyAll();
    }

    [Fact]
    public async Task SendAsync_WhenTokenCredentialThrows_DoesNotCallInnerHandler()
    {
        // Arrange
        var appSettings = new AppSettings
        {
            MicrosoftEntra = new MicrosoftEntra
            {
                Audience = "api://test-audience"
            }
        };

        var credential = new Mock<TokenCredential>(MockBehavior.Strict);
        credential
            .Setup(c => c.GetTokenAsync(It.IsAny<TokenRequestContext>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("no token"));

        var inner = new CountingHandler();

        var sut = new AuthHeadersHandler(credential.Object, appSettings)
        {
            InnerHandler = inner
        };

        using var invoker = new HttpMessageInvoker(sut);

        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/baz");

        // Act + Assert
        await Should.ThrowAsync<InvalidOperationException>(() => invoker.SendAsync(request, CancellationToken.None));

        inner.CallCount.ShouldBe(0);
        credential.VerifyAll();
    }

    private sealed class CaptureRequestHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public CaptureRequestHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        public HttpRequestMessage? Captured { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Captured = request;
            return Task.FromResult(_responseFactory(request));
        }
    }

    private sealed class CountingHandler : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}