using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsImport.Application.Contracts;

public interface ISponsorOrganisationService
{
    Task<SponsorOrganisation> DisableSponsorOrganisation(string rtsId);

    Task<SponsorOrganisation> EnableSponsorOrganisation(string rtsId);
}