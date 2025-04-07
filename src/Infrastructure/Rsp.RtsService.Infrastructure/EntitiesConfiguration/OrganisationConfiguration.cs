using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Infrastructure.EntitiesConfiguration;
public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
{
    public void Configure(EntityTypeBuilder<Organisation> builder)
    {
        builder
            .Property(p => p.Id)
            .HasColumnType("varchar(150)");

        builder
            .Property(p => p.Type)
            .HasColumnType("varchar(150)");

        builder
            .HasKey(nameof(Organisation.Id));

        builder
            .HasMany(x => x.Roles)
            .WithOne()
            .HasForeignKey(x => x.OrganisationId);

        //builder
        //    .HasOne(x => x.TypeEntity)
        //    .WithMany()
        //    .HasForeignKey(x => x.Type);
    }
}
