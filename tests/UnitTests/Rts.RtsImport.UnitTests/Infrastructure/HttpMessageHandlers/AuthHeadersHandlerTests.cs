using Moq;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Infrastructure.HttpMessageHandlers;
using Rts.RtsImport.UnitTests.Helpers;
using Shouldly;

namespace Rts.RtsImport.UnitTests.Infrastructure.HttpMessageHandlers;

public class RtsAuthHeadersHandlerTests : TestServiceBase<RtsAuthHeadersHandler>
{
    [Fact]
    public async Task SendAsync_ShouldAddAuthorizationHeader_WhenTokenIsAvailable()
    {
        // Arrange
        const string token = "test-token";

        Mocker
            .GetMock<ITokenService>()
            .Setup(service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        Sut.InnerHandler = new TestHttpMessageHandler(); // Simulate downstream handler

        var client = new HttpClient(Sut);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://fake.api/rts/data");

        // Act
        await client.SendAsync(request, CancellationToken.None);

        // Assert
        request.Headers.Authorization.ShouldNotBeNull();
        request.Headers.Authorization.Scheme.ShouldBe("Bearer");
        request.Headers.Authorization.Parameter.ShouldBe(token);
    }

    [Fact]
    public async Task SendAsync_ShouldNotAddAuthorizationHeader_WhenTokenIsNullOrEmpty()
    {
        // Arrange
        Mocker
            .GetMock<ITokenService>()
            .Setup(service => service.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        Sut.InnerHandler = new TestHttpMessageHandler(); // Simulate downstream handler

        var client = new HttpClient(Sut);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://fake.api/rts/data");

        // Act
        await client.SendAsync(request, CancellationToken.None);

        // Assert
        request.Headers.Authorization.ShouldBeNull();
    }
}