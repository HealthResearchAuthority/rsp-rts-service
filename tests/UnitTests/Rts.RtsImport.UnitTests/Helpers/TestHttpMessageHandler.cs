using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Rts.RtsImport.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
public class TestHttpMessageHandler
(
    Func<HttpRequestMessage, 
    HttpResponseMessage> handlerFunc
) : HttpMessageHandler
{
    public TestHttpMessageHandler() : this(_ => new HttpResponseMessage(HttpStatusCode.OK))
    {
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = handlerFunc(request);
        return Task.FromResult(response);
    }
}