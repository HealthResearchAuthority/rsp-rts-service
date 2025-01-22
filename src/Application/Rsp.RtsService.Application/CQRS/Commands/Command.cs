using MediatR;
using Rsp.RtsService.Application.DTOS.Requests;
using Rsp.RtsService.Application.DTOS.Responses;

namespace Rsp.RtsService.Application.CQRS.Commands;

// TODO: This is an example command for CQRS pattern
// Rename as appropriate
public class Command(CommandRequest request) : IRequest<CommandResponse>
{
    public CommandRequest Request { get; set; } = request;
}