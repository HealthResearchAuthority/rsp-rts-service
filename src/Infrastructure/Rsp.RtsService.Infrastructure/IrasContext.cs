using Microsoft.EntityFrameworkCore;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure.EntitiesConfiguration;

namespace Rsp.RtsService.Infrastructure;

public class IrasContext(DbContextOptions<IrasContext> options) : DbContext(options)
{
    public DbSet<SponsorOrganisation> SponsorOrganisations { get; set; }
    public DbSet<SponsorOrganisationAuditTrail> SponsorOrganisationsAuditTrail { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new SponsorOrganisationConfiguration());
        modelBuilder.ApplyConfiguration(new SponsorOrganisationAuditTrailConfiguration());
    }
}