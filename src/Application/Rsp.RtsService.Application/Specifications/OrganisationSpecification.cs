using Ardalis.Specification;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Specifications;

public class OrganisationSpecification : Specification<Organisation>
{
    /// <summary>
    /// Specification to get organisations by name
    /// </summary>
    /// <param name="name">Full or partial name of the organisation</param>    
    public OrganisationSpecification(string name, string? type = null)
    {
        var builder = Query.AsNoTracking();

        if (!string.IsNullOrEmpty(type))
        {
            builder.Where(x => x.Type == type);
        }
        
        builder
            .Where(entity => 
                entity.Name.Contains(name));
    }

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
}