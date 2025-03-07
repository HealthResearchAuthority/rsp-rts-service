﻿using Refit;
using Rsp.RtsImport.Application.DTO.Responses;

namespace Rsp.IrasPortal.Application.ServiceClients;

public interface IRtsAuthorisationServiceClient
{
    [Post("/api/auth/token")]
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    public Task<ApiResponse<RtsAuthResponseBody?>> GetBearerTokenAsync([Body] string body, CancellationToken cancellationToken);
}