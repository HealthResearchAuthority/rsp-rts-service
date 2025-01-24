using MediatR;
using Rsp.RtsService.Application.CQRS.Commands;
using Rsp.RtsService.Application.CQRS.Queries;
using Rsp.RtsService.Application.DTOS.Requests;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Rsp.RtsService.WebApi.Controllers;

// TODO: Example API Controller using CQRS pattern, update as needed
[ApiController]
[Route("[controller]")]
public class OrganisationsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Query organisations by complete or incomplete name. 
    /// </summary>
    [HttpGet("searchByName")]
    public async Task<QueryResponse> SearchByName(string name)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a single organisation by ID.
    /// </summary>
    [HttpGet("getById")]
    public async Task<QueryResponse> GetById(string id)
    {
        throw new NotImplementedException();
    }
}