using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Infrastructure.EntitiesConfiguration;
public class OrganisationTermsetConfiguration : IEntityTypeConfiguration<OrganisationTermset>
{
    public void Configure(EntityTypeBuilder<OrganisationTermset> builder)
    {
        builder.HasKey(nameof(OrganisationTermset.Id));
    }
}
