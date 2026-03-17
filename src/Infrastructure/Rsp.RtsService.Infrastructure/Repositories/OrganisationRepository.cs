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

        // perform sorting
        organisations = (sortField, sortDirection) switch
        {
            ("name", "asc") => [.. organisations.OrderBy(x => x.Name).ThenBy(x => x.Id)],
            ("name", "desc") => [.. organisations.OrderByDescending(x => x.Name).ThenBy(x => x.Id)],

            ("address", "asc") => [.. organisations.OrderBy(x => x.Address).ThenBy(x => x.Id)],
            ("address", "desc") => [.. organisations.OrderByDescending(x => x.Address).ThenBy(x => x.Id)],

            ("type", "asc") => [.. organisations.OrderBy(x => x.Type).ThenBy(x => x.Id)],
            ("type", "desc") => [.. organisations.OrderByDescending(x => x.Type).ThenBy(x => x.Id)],

            ("country", "asc") => [.. organisations.OrderBy(x => x.CountryName).ThenBy(x => x.Id)],
            ("country", "desc") => [.. organisations.OrderByDescending(x => x.CountryName).ThenBy(x => x.Id)],

            _ => [.. organisations.OrderBy(x => x.Name).ThenBy(x => x.Id)]
        };

        // Return the organisations and the total count
        return (organisations, count);
    }
}