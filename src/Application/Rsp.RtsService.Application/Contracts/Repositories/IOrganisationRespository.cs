using Ardalis.Specification;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Contracts.Repositories;

public interface IOrganisationRepository
{
    Task<(IEnumerable<Organisation>, int)> GetBySpecification(ISpecification<Organisation> specification, int pageIndex, int? pageSize);

    Task<(IEnumerable<Organisation>, int)> GetBySpecification(ISpecification<Organisation> specification, int pageIndex, int? pageSize, string sortField, string sortDirection);

    Task<Organisation?> GetById(ISpecification<Organisation> specification);
}