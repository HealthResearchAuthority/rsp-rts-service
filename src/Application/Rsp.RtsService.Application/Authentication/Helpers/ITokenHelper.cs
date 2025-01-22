using Microsoft.Extensions.Primitives;

namespace Rsp.RtsService.Application.Authentication.Helpers;

public interface ITokenHelper
{
    string DeBearerizeAuthToken(StringValues authToken);

    string BearerizeAuthToken(StringValues authToken);
}