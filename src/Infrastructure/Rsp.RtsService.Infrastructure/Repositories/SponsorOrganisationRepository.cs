using Microsoft.EntityFrameworkCore;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Infrastructure.Repositories;

public class SponsorOrganisationRepository(IrasContext irasContext) : ISponsorOrganisationsRepository
{
    public async Task<SponsorOrganisation> DisableSponsorOrganisation(string rtsId)
    {
        var sponsorOrganisation = await irasContext.SponsorOrganisations.FirstAsync(x => x.RtsId == rtsId);

        sponsorOrganisation.UpdatedDate = DateTime.Now;
        sponsorOrganisation.IsActive = false;

        await irasContext.SaveChangesAsync();

        return sponsorOrganisation;
    }

    public async Task<SponsorOrganisation> EnableSponsorOrganisation(string rtsId)
    {
        var sponsorOrganisation = await irasContext.SponsorOrganisations.FirstAsync(x => x.RtsId == rtsId);

        sponsorOrganisation.UpdatedDate = DateTime.Now;
        sponsorOrganisation.IsActive = true;

        await irasContext.SaveChangesAsync();

        return sponsorOrganisation;
    }
}