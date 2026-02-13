using Refit;
using Rsp.RtsService.Application.DTOS;

namespace Rsp.RtsImport.Application.ServiceClients;

/// <summary>
/// Interface to interact with Applications microservice
/// </summary>
public interface ISponsorOrganisationsServiceClient
{
    /// <summary>
    /// Disable a Sponsor Organisation by RTS ID
    /// </summary>
    [Get("/sponsororganisations/{rtsId}/enable")]
    Task<IApiResponse<SponsorOrganisationDto>> EnableSponsorOrganisation(string rtsId);

    /// <summary>
    /// Disable a Sponsor Organisation by RTS ID
    /// </summary>
    [Get("/sponsororganisations/{rtsId}/disable")]
    Task<IApiResponse<SponsorOrganisationDto>> DisableSponsorOrganisation(string rtsId);
}