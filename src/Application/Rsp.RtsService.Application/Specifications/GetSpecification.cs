using Ardalis.Specification;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Specifications;

// Supports specification pattern
// TODO: Example specification to use in the repository
public class GetSpecification : Specification<Entity>
{
    /// <summary>
    /// Defines a specification to return a single, all or a number of records
    /// </summary>
    /// <param name="id">Unique Id of the entity to get. Default: null for all records</param>
    /// <param name="records">Number of records to return. Default: 0 for all records</param>
    public GetSpecification(int? id = null, int records = 0)
    {
        Query
            .AsNoTracking()
            .Where(entity => entity.Id == id, id != null)
            .Skip(records, id == null && records == 0)
            .Take(records, id == null && records != 0);
    }

    /// <summary>
    /// Defines a specification to return a single, all or a number of records with specified criteria
    /// </summary>
    /// <param name="otherproperty">otherproperty of the entity</param>
    /// <param name="id">Unique Id of the entity to get. Default: null for all records</param>
    /// <param name="records">Number of records to return. Default: 0 for all records</param>
    public GetSpecification(string otherproperty, int? id = null, int records = 0)
    {
        Query
            .AsNoTracking()
            .Where(entity => entity.Id == id, id != null)
            //.Where(entity => entity.OtherProperty == otherproperty, id == null)
            .Skip(records, id == null && records == 0)
            .Take(records, id == null && records != 0);
    }
}