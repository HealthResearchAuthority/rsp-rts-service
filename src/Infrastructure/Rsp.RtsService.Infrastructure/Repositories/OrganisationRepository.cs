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
    ///     Searches for organisations using the provided specification and page size.
    /// </summary>
    /// <param name="pageIndex">Index (1-based) of page for paginated results.</param>
    /// <param name="pageSize">Optional maximum number of results to return.</param>
    /// <param name="specification">The specification that defines the search criteria.</param>
    public async Task<(IEnumerable<Organisation>, int)> GetBySpecification
    (
        ISpecification<Organisation> specification,
        int pageIndex,
        int? pageSize
    )
    {
        var count = await context
            .Organisation
            .WithSpecification(specification)
            .CountAsync();

        var query = context
            .Organisation
            .WithSpecification(specification);

        // Apply pagination if pageSize is specified
        if (pageSize.HasValue)
        {
            query = query
                .Skip((pageIndex - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }

        // Execute the query
        var organisations = await query.ToListAsync();

        // Return the organisations and the total count
        return (organisations, count);
    }

    /// <summary>
    ///     Searches for organisations using the provided specification and page size.
    /// </summary>
    /// <param name="pageIndex">Index (1-based) of page for paginated results.</param>
    /// <param name="pageSize">Optional maximum number of results to return.</param>
    /// <param name="specification">The specification that defines the search criteria.</param>
    public async Task<(IEnumerable<Organisation>, int)> GetBySpecification
    (
        ISpecification<Organisation> specification,
        int pageIndex,
        int? pageSize,
        string sortField,
        string sortDirection
    )
    {
        var baseQuery = context
            .Organisation
            .WithSpecification(specification);

        var count = await baseQuery.CountAsync();

        // Order by Name (DB level)
        var query = baseQuery
            .OrderBy(x => x.Name);

        // Apply pagination if pageSize is specified
        var topResults = pageSize.HasValue switch
        {
            true => await query
                .Skip((pageIndex - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToListAsync(),
            false => await query.ToListAsync()
        };

        // Sort those N in-memory
        var organisations = (sortField.ToLower(), sortDirection.ToLower()) switch
        {
            ("name", "asc") => topResults.OrderBy(x => x.Name),
            ("name", "desc") => topResults.OrderByDescending(x => x.Name),

            ("address", "asc") => topResults.OrderBy(x => x.Address),
            ("address", "desc") => topResults.OrderByDescending(x => x.Address),

            ("type", "asc") => topResults.OrderBy(x => x.Type),
            ("type", "desc") => topResults.OrderByDescending(x => x.Type),

            ("country", "asc") => topResults.OrderBy(x => x.CountryName),
            ("country", "desc") => topResults.OrderByDescending(x => x.CountryName),

            _ => topResults.OrderBy(x => x.Name)
        };

        return (organisations.ToList(), count);
    }
}