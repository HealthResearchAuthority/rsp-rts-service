using Mapster;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.Contracts.Services;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Application.Enums;
using Rsp.RtsService.Application.Specifications;

namespace Rsp.RtsService.Services;

public class OrganisationService(IOrganisationRepository repository) : IOrganisationService
{
    public async Task<GetOrganisationByIdDto> GetById(string id)
    {
        var record = await repository.GetById(new OrganisationSpecification(id));
        return record.Adapt<GetOrganisationByIdDto>();
    }

    /// <summary>
    /// Gets all organisations, with optional role filtering and paging.
    /// </summary>
    /// <param name="pageIndex">Index (1-based) of page for paginated results.</param>
    /// <param name="pageSize">Optional maximum number of results to return.</param>
    /// <param name="role">Optional role to filter organisations by.</param>
    /// <param name="sortOrder" >Sort order for the results, either ascending or descending.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<OrganisationSearchResponse> GetAll(int pageIndex, int? pageSize, string? role = null, SortOrder sortOrder = SortOrder.Ascending)
    {
        var (organisations, count) = await repository.GetBySpecification(new OrganisationSpecification(role!, sortOrder), pageIndex, pageSize);

        return new OrganisationSearchResponse
        {
            Organisations = organisations.Adapt<IEnumerable<SearchOrganisationDto>>(),
            TotalCount = count,
        };
    }

    /// <summary>
    /// Searches for organisations by name, with optional role filtering and paging.
    /// </summary>
    /// <param name="name">The name or partial name of the organisation to search for.</param>
    /// <param name="pageIndex">Index (1-based) of page for paginated results.</param>
    /// <param name="pageSize">Optional maximum number of results to return.</param>
    /// <param name="role">Optional role to filter organisations by.</param>
    /// <param name="sortOrder" >Sort order for the results, either ascending or descending.</param>
    public async Task<OrganisationSearchResponse> SearchByName(string name, int pageIndex, int? pageSize, string? role = null, SortOrder sortOrder = SortOrder.Ascending)
    {
        var (organisations, count) = await repository.GetBySpecification(new OrganisationSpecification(name, role!, sortOrder), pageIndex, pageSize);

        return new OrganisationSearchResponse
        {
            Organisations = organisations.Adapt<IEnumerable<SearchOrganisationDto>>(),
            TotalCount = count,
        };
    }
}