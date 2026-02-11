using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Contracts.Repositories;

public interface ISponsorOrganisationAuditTrailRepository
{
    Task LogSponsorOrganisationAuditTrail(SponsorOrganisation sponsorOrganisation, Organisation organisation);
}