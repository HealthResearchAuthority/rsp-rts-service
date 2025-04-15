using Microsoft.EntityFrameworkCore;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure.EntitiesConfiguration;

namespace Rsp.RtsService.Infrastructure;

public class RtsDbContext(DbContextOptions<RtsDbContext> options) : DbContext(options)
{
    public DbSet<Metadata> Metadata { get; set; }
    public DbSet<Organisation> Organisation { get; set; }
    public DbSet<OrganisationRole> OrganisationRole { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrganisationConfiguration());
        modelBuilder.ApplyConfiguration(new OrganisationRoleConfiguration());
        modelBuilder.ApplyConfiguration(new MetadataConfiguration());
    }
}