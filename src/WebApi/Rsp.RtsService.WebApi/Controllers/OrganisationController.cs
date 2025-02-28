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
    public async Task<ActionResult<IEnumerable<OrganisationSearchResult>>> SearchByName(string name, int pageSize = 5, string? role = null)
    {
        if (name.Length < 3)
        {
            return BadRequest("Name needs to include minimum 3 characters");
        }

        var organisations = await orgService.SearchByName(name, pageSize, role);

        var result = organisations.Select(x =>
            new OrganisationSearchResult
            {
                Id = x.Id,
                Name = x.Name!
            });

        return Ok(result);
    }

    /// <summary>
    /// Get a single organisation by ID.
    /// </summary>
    [HttpGet("getById")]
    public async Task<ActionResult<Organisation>> GetById(string id)
    {
        var record = await orgService.GetById(id);

        return Ok(record);
    }
}