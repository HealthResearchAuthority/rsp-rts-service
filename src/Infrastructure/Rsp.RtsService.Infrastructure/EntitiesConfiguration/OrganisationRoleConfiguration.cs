using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Infrastructure.EntitiesConfiguration;

public class OrganisationRoleConfiguration : IEntityTypeConfiguration<OrganisationRole>
{
    public void Configure(EntityTypeBuilder<OrganisationRole> builder)
    {
        builder
            .Property(p => p.Id)
            .HasColumnType("varchar(150)");

        builder
            .Property(p => p.Scoper)
            .HasColumnType("varchar(150)");

        builder
            .Property(p => p.OrganisationId)
            .HasColumnType("varchar(150)");

        builder.HasKey(c => new { c.Id, c.OrganisationId, c.Scoper, c.CreatedDate });
    }
}