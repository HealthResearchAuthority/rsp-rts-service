using MediatR;
using Rsp.RtsService.Application.CQRS.Commands;
using Rsp.RtsService.Application.CQRS.Queries;
using Rsp.RtsService.Application.DTOS.Requests;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Rsp.RtsService.Application.Contracts.Services;

namespace Rsp.RtsService.WebApi.Controllers;

// TODO: Example API Controller using CQRS pattern, update as needed
[ApiController]
[Route("[controller]")]
public class OrganisationsController(IOrganisationService orgService) : ControllerBase
{
    /// <summary>
    /// Query organisations by complete or partial name. 
    /// </summary>
    [HttpGet("searchByName")]
    public async Task<ActionResult<IEnumerable<OrganisationSearchResult>>> SearchByName(string name, string? type = null)
    {
        try
        {
            if (name.Length < 3)
                return BadRequest("Name needs to include minimum 3 characters");

            var result = await orgService.SearchByName(name, type);

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