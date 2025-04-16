using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Refit;
using Rsp.RtsImport.Application.DTO.Responses;
using Rsp.RtsImport.Application.ServiceClients;
using Rsp.RtsImport.Application.Settings;
using Rsp.RtsImport.Infrastructure.HttpMessageHandlers;
using Rts.RtsImport.UnitTests.Helpers;
using Shouldly;

namespace Rts.RtsImport.UnitTests.Infrastructure.HttpMessageHandlers;
public class RtsAuthHeadersHandlerTests :TestServiceBase
{
    [Fact]
    public async Task SendAsync_AddsBearerToken_WhenNotAuthRequest()
    {
        // Arrange
        var expectedToken = "fake-token";

        var mockAuthClient = Mocker.GetMock<IRtsAuthorisationServiceClient>();
        mockAuthClient
            .Setup(x => x.GetBearerTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))!
            .ReturnsAsync(new ApiResponse<RtsAuthResponse>(
                new HttpResponseMessage(HttpStatusCode.OK),
                new RtsAuthResponse { AccessToken = expectedToken },
                new RefitSettings()
            ));

        var appSettings = new AppSettings
        {
            RtsApiClientId = "test-client",
            RtsApiClientSecret = "test-secret"
        };
        Mocker.Use(Options.Create(appSettings));

        var handler = Mocker.CreateInstance<RtsAuthHeadersHandler>();
        handler.InnerHandler = new TestHttpMessageHandler(); // Simulate downstream handler

        var client = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://fake.api/rts/data");

        // Act
        var response = await client.SendAsync(request);

        // Assert
        request.Headers.Authorization.ShouldNotBeNull();
        request.Headers.Authorization!.Scheme.ShouldBe("Bearer");
        request.Headers.Authorization.Parameter.ShouldBe(expectedToken);
    }


}
