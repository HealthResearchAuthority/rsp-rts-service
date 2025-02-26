using Microsoft.AspNetCore.Mvc;
using Rsp.RtsService.Application.Contracts.Services;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganisationsController(IOrganisationService orgService) : ControllerBase
{
    /// <summary>
    /// Query organisations by complete or partial name.
    /// </summary>
    [HttpGet("searchByName")]
    public async Task<ActionResult<IEnumerable<OrganisationSearchResult>>> SearchByName(string name, string? type = null, string? role = null)
    {
        try
        {
            if (name.Length < 3)
                return BadRequest("Name needs to include minimum 3 characters");

            var result = await orgService.SearchByName(name, type, role);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// Get a single organisation by ID.
    /// </summary>
    [HttpGet("getById")]
    public async Task<ActionResult<Organisation>> GetById(string id)
    {
        try
        {
            return Ok(await orgService.GetById(id));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}