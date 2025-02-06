using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure.EntitiesConfiguration;

namespace Rsp.RtsService.Infrastructure;
public class RtsDbContext(DbContextOptions<RtsDbContext> options) : DbContext(options)
{
    public DbSet<Organisation> Organisation { get; set; }
    public DbSet<OrganisationRole> OrganisationRole { get; set; }
    public DbSet<OrganisationTermset> OrganisationTermset { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrganisationConfiguration());
        modelBuilder.ApplyConfiguration(new OrganisationRoleConfiguration());
        modelBuilder.ApplyConfiguration(new OrganisationTermsetConfiguration());
    }
}
