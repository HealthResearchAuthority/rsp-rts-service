using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Infrastructure.EntitiesConfiguration;

public class SponsorOrganisationAuditTrailConfiguration : IEntityTypeConfiguration<SponsorOrganisationAuditTrail>
{
    public void Configure(EntityTypeBuilder<SponsorOrganisationAuditTrail> builder)
    {
        builder.HasKey(rb => rb.Id);

        builder
            .Property(rb => rb.Description)
            .IsRequired();

        builder
            .Property(x => x.User)
            .IsRequired()
            .HasMaxLength(250);

        builder
            .Property(x => x.DateTimeStamp)
            .IsRequired();
    }
}