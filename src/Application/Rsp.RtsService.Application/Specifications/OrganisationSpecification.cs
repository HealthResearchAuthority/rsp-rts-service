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
        Query
            .AsNoTracking()
            .Where(entity => entity.Id == id)
            .Include(x => x.Roles);
    }

    public OrganisationSpecification(string name, int pageSize, string roleId)
    {
        Query.AsNoTracking();

        if (!string.IsNullOrEmpty(roleId))
        {
            Query.Where(x => x.Roles.Any(x => x.Id == roleId));
        }

        Query.Where(x => x.Name != null && x.Name.Contains(name) && x.Status == true)
            .Take(pageSize);
    }
}