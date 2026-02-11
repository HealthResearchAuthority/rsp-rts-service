using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.Specifications;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsImport.Services;

public class SponsorOrganisationService(
    ISponsorOrganisationsRepository sponsorOrganisationsRepository,
    ISponsorOrganisationAuditTrailRepository sponsorOrganisationAuditTrailRepository, IOrganisationRepository organisationRepository)
    : ISponsorOrganisationService
{
    public async Task<SponsorOrganisation> DisableSponsorOrganisation(string rtsId)
    {
        var sponsorOrganisation = await sponsorOrganisationsRepository.DisableSponsorOrganisation(rtsId);
        var organisation = await organisationRepository.GetById(new OrganisationSpecification(rtsId));

        await sponsorOrganisationAuditTrailRepository.LogSponsorOrganisationAuditTrail(sponsorOrganisation, organisation);

        return sponsorOrganisation;
    }

    public async Task<SponsorOrganisation> EnableSponsorOrganisation(string rtsId)
    {
        var sponsorOrganisation = await sponsorOrganisationsRepository.EnableSponsorOrganisation(rtsId);
        var organisation = await organisationRepository.GetById(new OrganisationSpecification(rtsId));

        await sponsorOrganisationAuditTrailRepository.LogSponsorOrganisationAuditTrail(sponsorOrganisation, organisation);

        return sponsorOrganisation;
    }
}