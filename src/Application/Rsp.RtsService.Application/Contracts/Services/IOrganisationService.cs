using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Application.Enums;

namespace Rsp.RtsService.Application.Contracts.Services;

/// <summary>
/// Interface to create/read/update the records in the database
/// </summary>
public interface IOrganisationService
{
    /// <summary>
    /// Get organisation by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    /// <returns>The organisation entity if found; otherwise, null.</returns>
    Task<GetOrganisationByIdDto> GetById(string id);

    /// <summary>
    /// Get all organisations
    /// </summary>
    /// <param name="pageIndex">Index (1-based) of page for paginated results.</param>
    /// <param name="pageSize">Optional maximum number of results to return.</param>
    /// <param name="role">Optional role to filter organisations by.</param>
    /// <param name="sortOrder" >Sort order for the results, either ascending or descending.</param>
    /// <returns></returns>
    Task<OrganisationSearchResponse> GetAll(int pageIndex, int? pageSize, string? role = null, SortOrder sortOrder = SortOrder.Ascending);

    /// <summary>
    /// Searches for organisations by name, with optional role filtering and paging.
    /// </summary>
    /// <param name="name">The name or partial name of the organisation to search for.</param>
    /// <param name="pageIndex">Index (1-based) of page for paginated results.</param>
    /// <param name="pageSize">Optional maximum number of results to return.</param>
    /// <param name="role">Optional role to filter organisations by.</param>
    /// <param name="sortOrder" >Sort order for the results, either ascending or descending.</param>
    /// <returns>A collection of organisation search results.</returns>
    Task<OrganisationSearchResponse> SearchByName(string name, int pageIndex, int? pageSize, string? role = null, SortOrder sortOrder = SortOrder.Ascending);
}