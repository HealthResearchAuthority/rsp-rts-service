using Ardalis.Specification;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Specifications;

public class OrganisationRoleSpecification : Specification<OrganisationRole>
{
    public OrganisationRoleSpecification(string name, string? type = null, string? role = null)
    {
        var builder = Query.AsNoTracking();

        // apply role filter if present
        if (!string.IsNullOrEmpty(role))
        {
            builder.Where(r => r.Id == role);
        }

        // apply type filter if present
        if (!string.IsNullOrEmpty(type))
        {
            builder.Where(x => x.Organisation.Type == type);
        }

        builder.Where(x => x.Organisation.Name.Contains(name));
    }
}