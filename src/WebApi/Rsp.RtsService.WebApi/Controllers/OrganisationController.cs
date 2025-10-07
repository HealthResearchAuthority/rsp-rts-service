using Microsoft.AspNetCore.Mvc;
using Rsp.RtsService.Application.Contracts.Services;
using Rsp.RtsService.Application.DTOS.Responses;

[ApiController]
[Route("[controller]")]
public class OrganisationsController(IOrganisationService orgService) : ControllerBase
{
    /// <summary>
    ///     Query organisations by complete or partial name.
    /// </summary>
    /// <param name="name">The name or partial name of the organisation to search for.</param>
    /// <param name="pageIndex">1-based index of the page to retrieve. Must be greater than 0.</param>
    /// <param name="pageSize">Optional number of items per page. If null, returns all matches. Must be &gt; 0 if specified.</param>
    /// <param name="role">Optional role to filter organisations by.</param>
    /// <param name="countries">
    ///     Optional repeated query param(s) for country filter, e.g. ?countries=England&amp;
    ///     countries=Wales.
    /// </param>
    /// <param name="sort">Sort direction: "asc" or "desc".</param>
    /// <param name="sortField">Sort field: "name", "country", or "isactive".</param>
    [HttpGet("searchByName")]
    public async Task<ActionResult<OrganisationSearchResponse>> SearchByName(
        string name,
        int pageIndex = 1,
        int? pageSize = null,
        string? role = null,
        [FromQuery] string[]? countries = null,
        string sort = "asc",
        string sortField = "name")
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 3)
        {
            return BadRequest("Name needs to include minimum 3 characters");
        }

        if (pageIndex <= 0)
        {
            return BadRequest("pageIndex must be greater than 0 if specified.");
        }

        if (pageSize.HasValue && pageSize <= 0)
        {
            return BadRequest("pageSize must be greater than 0 if specified.");
        }

        var countryFilter = countries?
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var result = await orgService.SearchByName(
            name,
            pageIndex,
            pageSize,
            role,
            countryFilter,
            sortField,
            sort);

        return Ok(result);
    }

    /// <summary>
    ///     Retrieves a paginated list of organisations, with optional role/country filtering and sorting.
    /// </summary>
    /// <param name="pageIndex">1-based index of the page to retrieve. Must be greater than 0.</param>
    /// <param name="pageSize">Optional number of items per page. If null, returns all matches. Must be &gt; 0 if specified.</param>
    /// <param name="role">Optional role to filter organisations by.</param>
    /// <param name="countries">
    ///     Optional repeated query param(s) for country filter, e.g. ?countries=England&amp;
    ///     countries=Wales.
    /// </param>
    /// <param name="sort">Sort direction: "asc" or "desc".</param>
    /// <param name="sortField">Sort field: "name", "country", or "isactive".</param>
    [HttpGet("getAll")]
    public async Task<ActionResult<OrganisationSearchResponse>> GetAll(
        int pageIndex = 1,
        int? pageSize = null,
        string? role = null,
        [FromQuery] string[]? countries = null,
        string sort = "asc",
        string sortField = "name")
    {
        if (pageIndex <= 0)
        {
            return BadRequest("pageIndex must be greater than 0 if specified.");
        }

        if (pageSize.HasValue && pageSize <= 0)
        {
            return BadRequest("pageSize must be greater than 0 if specified.");
        }

        var countryFilter = countries?
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var result = await orgService.GetAll(
            pageIndex,
            pageSize,
            role,
            countryFilter,
            sortField,
            sort);

        return Ok(result);
    }

    /// <summary>
    ///     Get a single organisation by ID.
    /// </summary>
    [HttpGet("getById")]
    public async Task<ActionResult<GetOrganisationByIdDto>> GetById(string id)
    {
        var record = await orgService.GetById(id);
        return Ok(record);
    }
}