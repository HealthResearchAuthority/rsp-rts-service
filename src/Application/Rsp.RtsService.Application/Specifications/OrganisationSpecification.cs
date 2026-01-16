using Ardalis.Specification;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Specifications;

public class OrganisationSpecification : Specification<Organisation>
{
    public OrganisationSpecification(string id)
    {
        Query
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Include(x => x.Roles);
    }

    /// <summary>
    /// Main overload: filters by optional name, roleId, and multiple CountryName values. Supports
    /// dynamic sorting using tuple-switch syntax.
    /// </summary>
    public OrganisationSpecification(
        string? name,
        string? roleId,
        string[]? countryNames,
        string sortField = "name",
        string sortDirection = "asc")
    {
        Query.AsNoTracking();

        // Organisation must be active
        Query.Where(x => x.Status == true);

        // Optional role filter
        if (!string.IsNullOrWhiteSpace(roleId))
        {
            Query.Where(x => x.Roles.Any(r => r.Id == roleId));
        }

        // Optional name filter (case-insensitive via ToLower)
        if (!string.IsNullOrWhiteSpace(name))
        {
            var term = name.ToLower();
            Query.Where(x => x.Name != null && x.Name.ToLower().Contains(term));
        }

        // Multi-country filter (scalar CountryName)
        if (countryNames is { Length: > 0 })
        {
            var set = countryNames
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (set.Count > 0)
            {
                Query.Where(x => x.CountryName != null && set.Contains(x.CountryName));
            }
        }

        // Only include active roles in the result payload
        Query.Where(x => x.Roles.Any(r =>
            r.Status != null &&
            r.Status.ToLower() == "active"));

        _ = (sortField, sortDirection) switch
        {
            ("name", "asc") => Query.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", "desc") => Query.OrderByDescending(x => x.Name).ThenBy(x => x.Id),

            ("country", "asc") => Query.OrderBy(x => x.CountryName).ThenBy(x => x.Id),
            ("country", "desc") => Query.OrderByDescending(x => x.CountryName).ThenBy(x => x.Id),

            ("isactive", "asc") => Query.OrderByDescending(x => x.Status),
            ("isactive", "desc") => Query.OrderBy(x => x.Status),

            _ => Query.OrderBy(x => x.Name).ThenBy(x => x.Id)
        };
    }

    /// <summary>
    /// Optional overload for IEnumerable&lt;string&gt; inputs (redirects to main string[] constructor).
    /// </summary>
    public OrganisationSpecification(
        string? name,
        string? roleId,
        IEnumerable<string>? countryNames,
        string sortField = "name",
        string sortDirection = "asc")
        : this(name, roleId, countryNames?.ToArray(), sortField, sortDirection)
    {
    }
}