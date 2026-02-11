using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Infrastructure.Repositories;

public class SponsorOrganisationAuditTrailRepository(IrasContext irasContext) : ISponsorOrganisationAuditTrailRepository
{
    public async Task LogSponsorOrganisationAuditTrail(
        SponsorOrganisation sponsorOrganisation, Organisation organisation)

    {
        var auditTrail = new SponsorOrganisationAuditTrail
        {
            DateTimeStamp = DateTime.UtcNow,
            RtsId = sponsorOrganisation.RtsId,
            SponsorOrganisationId = sponsorOrganisation.Id,
            User = "System generated",
            Description = GenerateDescription(sponsorOrganisation, organisation)
        };

        irasContext.SponsorOrganisationsAuditTrail.Add(auditTrail);

        irasContext.SaveChangesAsync();
    }

    private static string GenerateDescription(
        SponsorOrganisation sponsorOrganisation, Organisation organisation)
    {
        var newStatus = sponsorOrganisation.IsActive ? "enabled" : "disabled";
        return $"{organisation.Name} {newStatus}";
    }
}