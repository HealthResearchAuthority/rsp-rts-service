using Ardalis.Specification;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Contracts.Repositories;

public interface IRepository
{
    /// <summary>
    /// Return a single entity from the database
    /// </summary>
    Task<Entity> Get(ISpecification<Entity> specification);

    /// <summary>
    /// Adds a new entity to the database
    /// </summary>
    /// <param name="entity">The entity values</param>
    Task<Entity> Create(Entity entity);

    /// <summary>
    /// Update the values of an entity in the database
    /// </summary>
    /// <param name="id">The entity id</param>
    /// <param name="entity">The entity to update</param>
    Task<Entity> Update(int id, Entity entity);
}