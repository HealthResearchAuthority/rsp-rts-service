using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Infrastructure.EntitiesConfiguration;

public class SponsorOrganisationConfiguration : IEntityTypeConfiguration<SponsorOrganisation>
{
    public void Configure(EntityTypeBuilder<SponsorOrganisation> builder)
    {
        builder.HasKey(rb => rb.Id);

        builder.Property(rb => rb.CreatedBy)
                   .HasMaxLength(250);

        builder.Property(rb => rb.UpdatedBy)
            .HasMaxLength(250);
    }
}