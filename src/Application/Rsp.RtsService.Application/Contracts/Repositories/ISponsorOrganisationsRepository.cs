using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Contracts.Repositories;

public interface ISponsorOrganisationsRepository
{
    Task<SponsorOrganisation> DisableSponsorOrganisation(string rtsId);

    Task<SponsorOrganisation> EnableSponsorOrganisation(string rtsId);
}