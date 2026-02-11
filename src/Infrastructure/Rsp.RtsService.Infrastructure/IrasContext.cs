using Microsoft.EntityFrameworkCore;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Infrastructure;

public class IrasContext(DbContextOptions<IrasContext> options) : DbContext(options)
{
    public DbSet<SponsorOrganisation> SponsorOrganisations { get; set; }
    public DbSet<SponsorOrganisationAuditTrail> SponsorOrganisationsAuditTrail { get; set; }
}