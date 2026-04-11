using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Specifications;

public class OrganisationsSearchSpecification : Specification<Organisation>
{
    /// <summary>
    /// Main overload: filters by optional name, roleId, and multiple CountryName values. Supports
    /// dynamic sorting using tuple-switch syntax.
    /// </summary>
    public OrganisationsSearchSpecification
    (
        OrganisationsSearchRequest request
    )
    {
        Query.AsNoTracking();

        // Optional role filter
        if (request.ExcludingRoles.Count > 0)
        {
            Query.Where
            (
                x => !x.Roles.Any(r => request.ExcludingRoles.Contains(r.Id))
            );
        }

        // Optional exclusion of specific organisation IDs
        if (request.ExcludedOrganisationIds.Count > 0)
        {
            Query.Where
            (
                x => !request.ExcludedOrganisationIds.Contains(x.Id)
            );
        }

        // Optional name filter
        if (!string.IsNullOrWhiteSpace(request.SearchNameTerm))
        {
            var terms = request.SearchNameTerm
               .Split(' ', StringSplitOptions.RemoveEmptyEntries)
               .AsEnumerable();

            foreach (var term in terms)
            {
                var t = term; // avoid closure issues

                Query.Where
                (
                    x => EF.Functions.Like(x.Name, $"%{t}%")
                );
            }
        }

        // Multi-country filter
        if (request.Countries.Count > 0)
        {
            Query.Where(x => x.CountryName != null && request.Countries.Contains(x.CountryName));
        }

        // Multi-types filter
        if (request.OrganisationTypes.Count > 0)
        {
            Query.Where(x => x.Type != null && request.OrganisationTypes.Contains(x.Type));
        }

        // Multi-statuses filter
        if (request.OrganisationStatuses.Count > 0)
        {
            Query.Where(x => request.OrganisationStatuses.Contains(x.Status));
        }
    }
}