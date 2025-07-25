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
    /// Query organisations by complete or partial name..
    /// </summary>
    [HttpGet("searchByName")]
    public async Task<ActionResult<OrganisationSearchResponse>> SearchByName(string name, int pageSize = 5, string? role = null, string sort = "asc")
    {
        if (name.Length < 3)
        {
            return BadRequest("Name needs to include minimum 3 characters");
        }

        var sortOrder = sort switch
        {
            "asc" => SortOrder.Ascending,
            "desc" => SortOrder.Descending,
            _ => SortOrder.Ascending
        };

        var organisations = await orgService.SearchByName(name, pageSize, role, sortOrder);

        return Ok(organisations);
    }

    /// <summary>
    /// Get all organisations.
    /// </summary>
    [HttpGet("getAll")]
    public async Task<ActionResult<OrganisationSearchResponse>> GetAll(int pageSize = 5, string? role = null, string sort = "asc")
    {
        var sortOrder = sort switch
        {
            "asc" => SortOrder.Ascending,
            "desc" => SortOrder.Descending,
            _ => SortOrder.Ascending
        };

        var organisations = await orgService.GetAll(pageSize, role, sortOrder);

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