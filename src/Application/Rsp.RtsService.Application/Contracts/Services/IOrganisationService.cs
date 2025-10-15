using Rsp.RtsService.Application.DTOS.Responses;

namespace Rsp.RtsService.Application.Contracts.Services;

/// <summary>
///     Interface to create/read/update the records in the database
/// </summary>
public interface IOrganisationService
{
    /// <summary>
    ///     Get organisation by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the organisation.</param>
    /// <returns>The organisation entity if found; otherwise, null.</returns>
    Task<GetOrganisationByIdDto> GetById(string id);

    /// <summary>
    ///     Gets all organisations, with optional role filtering, multi-country filter, sorting, and paging.
    /// </summary>
    /// <param name="pageIndex">Index (1-based) of page for paginated results.</param>
    /// <param name="pageSize">Optional maximum number of results to return.</param>
    /// <param name="role">Optional role to filter organisations by.</param>
    /// <param name="countries">Optional list of CountryName values to filter by.</param>
    /// <param name="sortField">"name", "country", or "isactive". Defaults to "name".</param>
    /// <param name="sortDirection">"asc" or "desc". Defaults to "asc".</param>
    Task<OrganisationSearchResponse> GetAll(
        int pageIndex,
        int? pageSize,
        string? role = null,
        string[]? countries = null,
        string sortField = "name",
        string sortDirection = "asc");

    /// <summary>
    ///     Searches for organisations by name, with optional role filtering, multi-country filter, sorting, and paging.
    /// </summary>
    /// <param name="name">The name or partial name of the organisation to search for.</param>
    /// <param name="pageIndex">Index (1-based) of page for paginated results.</param>
    /// <param name="pageSize">Optional maximum number of results to return.</param>
    /// <param name="role">Optional role to filter organisations by.</param>
    /// <param name="countries">Optional list of CountryName values to filter by.</param>
    /// <param name="sortField">"name", "country", or "isactive". Defaults to "name".</param>
    /// <param name="sortDirection">"asc" or "desc". Defaults to "asc".</param>
    Task<OrganisationSearchResponse> SearchByName(
        string name,
        int pageIndex,
        int? pageSize,
        string? role = null,
        string[]? countries = null,
        string sortField = "name",
        string sortDirection = "asc");
}