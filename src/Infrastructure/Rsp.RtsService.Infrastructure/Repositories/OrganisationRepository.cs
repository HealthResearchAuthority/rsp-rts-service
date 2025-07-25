using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Infrastructure.Repositories;

public class OrganisationRepository(RtsDbContext context) : IOrganisationRepository
{
    public async Task<Organisation?> GetById(ISpecification<Organisation> specification)
    {
        var record = await context
            .Organisation
            .WithSpecification(specification)
            .FirstOrDefaultAsync();

        return record;
    }

    /// <summary>
    /// Searches for organisations using the provided specification and page size.
    /// </summary>
    /// <param name="pageSize">The maximum number of records to return.</param>
    /// <param name="specification">The specification that defines the search criteria.</param>
    public async Task<(IEnumerable<Organisation>, int)> GetBySpecification(ISpecification<Organisation> specification, int pageSize)
    {
        /// count the total number of records that match the specification
        var count = await context
            .Organisation
            .WithSpecification(specification)
            .CountAsync();

        // only take the specified number of records
        var organisations = await context
            .Organisation
            .WithSpecification(specification)
            .Take(pageSize)
            .ToListAsync();

        // return the organisations and the count
        return (organisations, count);
    }
}