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
public class ExampleController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Produces<Entity>]
    public async Task<QueryResponse> QueryRequest(int id)
    {
        var query = new Query(id);

        return await mediator.Send(query);
    }

    [HttpPost]
    public async Task<CommandResponse> CommandRequest(CommandRequest commandRequest)
    {
        var request = new Command(commandRequest);

        return await mediator.Send(request);
    }
}