using Microsoft.AspNetCore.Mvc;
using Rsp.RtsService.Application.Contracts.Services;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Application.Enums;

namespace Rsp.RtsService.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganisationsController(IOrganisationService orgService) : ControllerBase
{
    /// <summary>
    /// Query organisations by complete or partial name
    /// </summary>
    /// <param name="name">The name or partial name of the organisation to search for.</param>
    /// <param name="pageIndex">1-based index of the page to retrieve. Must be greater than 0.</param>
    /// <param name="pageSize">Optional number of items per page. If null, all matching organisations are returned. Must be greater than 0 if specified.</param>
    /// <param name="role">Optional role to filter organisations by.</param>
    /// <param name="sort">Sort order for the results, either ascending or descending.</param>
    /// <returns></returns>
    [HttpGet("searchByName")]
    public async Task<ActionResult<OrganisationSearchResponse>> SearchByName(string name, int pageIndex = 1, int? pageSize = null, string? role = null, string sort = "asc")
    {
        if (name.Length < 3)
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

        var sortOrder = sort switch
        {
            "asc" => SortOrder.Ascending,
            "desc" => SortOrder.Descending,
            _ => SortOrder.Ascending
        };

        var organisations = await orgService.SearchByName(name, pageIndex, pageSize, role, sortOrder);

        return Ok(organisations);
    }

    /// <summary>
    /// Retrives a paginated list of all organisations
    /// </summary>
    /// <param name="pageIndex">1-based index of the page to retrieve. Must be greater than 0.</param>
    /// <param name="pageSize">Optional number of items per page. If null, all matching organisations are returned. Must be greater than 0 if specified.</param>
    /// <param name="role">Optional role to filter organisations by.</param>
    /// <param name="sort">Sort order for the results, either ascending or descending.</param>
    /// <returns></returns>
    [HttpGet("getAll")]
    public async Task<ActionResult<OrganisationSearchResponse>> GetAll(int pageIndex = 1, int? pageSize = null, string? role = null, string sort = "asc")
    {
        if (pageIndex <= 0)
        {
            return BadRequest("pageIndex must be greater than 0 if specified.");
        }
        if (pageSize.HasValue && pageSize <= 0)
        {
            return BadRequest("pageSize must be greater than 0 if specified.");
        }

        var sortOrder = sort switch
        {
            "asc" => SortOrder.Ascending,
            "desc" => SortOrder.Descending,
            _ => SortOrder.Ascending
        };

        var organisations = await orgService.GetAll(pageIndex, pageSize, role, sortOrder);

        return Ok(organisations);
    }

    /// <summary>
    /// Get a single organisation by ID.
    /// </summary>
    [HttpGet("getById")]
    public async Task<ActionResult<GetOrganisationByIdDto>> GetById(string id)
    {
        var record = await orgService.GetById(id);

        return Ok(record);
    }
}