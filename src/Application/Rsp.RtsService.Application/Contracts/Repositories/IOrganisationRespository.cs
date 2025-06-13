using Ardalis.Specification;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Contracts.Repositories;

public interface IOrganisationRepository
{
    Task<(IEnumerable<Organisation>, int)> SearchByName(ISpecification<Organisation> specification, int pageSize);

    Task<Organisation?> GetById(ISpecification<Organisation> specification);
}