using Ardalis.Specification;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Contracts.Repositories;

public interface IOrganisationRoleRepository
{
    Task<IEnumerable<OrganisationSearchResult>> SearchByName(ISpecification<OrganisationRole> specification);
}