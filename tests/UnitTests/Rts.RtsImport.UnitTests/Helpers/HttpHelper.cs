using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Rts.RtsImport.UnitTests.Helpers;

public static class HttpHelper
{
    public static HttpRequest CreateHttpRequest(Dictionary<string, string>? query = null)
    {
        var context = new DefaultHttpContext();
        var request = context.Request;

        if (query != null)
        {
            var queryCollection = new QueryCollection(
                query.ToDictionary(
                    k => k.Key,
                    v => new StringValues(v.Value)
                )
            );

            request.Query = queryCollection;
        }

        return request;
    }
}