using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Infrastructure.Repositories;

public class OrganisationRepository(RtsDbContext context) : IOrganisationRepository
{
    public async Task<Organisation> GetById(ISpecification<Organisation> specification)
    {
        var record = await context
            .Organisation
            .WithSpecification(specification)
            .FirstOrDefaultAsync();

        return record;
    }

    public async Task<IEnumerable<Organisation>> SearchByName(ISpecification<Organisation> specification)
    {
        var result = await context
            .Organisation
            .WithSpecification(specification)
            .Take(20).ToListAsync();

        return result;
    }
}