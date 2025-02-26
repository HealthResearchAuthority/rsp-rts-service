using Ardalis.Specification;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Specifications;

public class OrganisationSpecification : Specification<Organisation>
{
    /// <summary>
    /// Specification to get organisations by ID
    /// </summary>
    /// <param name="id">Id of the organisatio record</param>
    public OrganisationSpecification(string id)
    {
        var builder = Query
            .AsNoTracking()
            .Where(entity => entity.Id == id)
            .Include(x => x.Roles)
            .Include(x => x.TypeEntity);
    }

    public OrganisationSpecification(string name, string roleId)
    {
        var builder = Query.AsNoTracking();

        if (!string.IsNullOrEmpty(roleId))
        {
            builder.Where(x => x.Roles.Any(x => x.Id == roleId));
        }

        builder.Where(x => x.Name != null && x.Name.Contains(name));
    }
}