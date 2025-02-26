using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Infrastructure.Repositories;

public class OrganisationRoleRepository(RtsDbContext context) : IOrganisationRoleRepository
{
    public async Task<IEnumerable<OrganisationSearchResult>> SearchByName(ISpecification<OrganisationRole> specification)
    {
        var result = await context
            .OrganisationRole
            .WithSpecification(specification)
            .Take(20)
            .Select(x => new OrganisationSearchResult
            {
                Id = x.Id,
                Name = x.Organisation.Name!
            }).ToListAsync();

        return result;
    }
}