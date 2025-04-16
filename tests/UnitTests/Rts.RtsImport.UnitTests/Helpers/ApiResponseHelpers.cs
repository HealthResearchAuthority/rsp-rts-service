using System.Diagnostics.CodeAnalysis;
using System.Net;
using Refit;
using Rsp.RtsImport.Application.DTO.Responses;

namespace Rts.RtsImport.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
public static class ApiResponseHelpers
{
    public static ApiResponse<RtsOrganisationsAndRolesResponse> CreateRtsOrganisationsAndRolesResponse(RtsOrganisationsAndRolesResponse content)
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
        var refitSettings = new RefitSettings(); // Can be empty for test

        return new ApiResponse<RtsOrganisationsAndRolesResponse>(
            httpResponse,
            content,
            refitSettings
        );
    }
}