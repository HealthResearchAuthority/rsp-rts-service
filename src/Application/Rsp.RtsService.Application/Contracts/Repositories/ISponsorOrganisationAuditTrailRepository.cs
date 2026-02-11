using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Contracts.Repositories;

public interface ISponsorOrganisationAuditTrailRepository
{
    Task<SponsorOrganisationAuditTrail> LogSponsorOrganisationAuditTrail(SponsorOrganisation sponsorOrganisation, Organisation organisation);
}