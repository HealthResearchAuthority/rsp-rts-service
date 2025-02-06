using MediatR;
using Rsp.RtsService.Application.Contracts.Services;
using Rsp.RtsService.Application.CQRS.Commands;
using Rsp.RtsService.Application.DTOS.Responses;
using Microsoft.Extensions.Logging;

namespace Rsp.RtsService.Application.CQRS.Handlers.CommandHandlers;

// TODO: This is just an example of a command handler which takes logger and IRtsService
// as dependencies, please rename/modify as appropriate
public class CommandHandler(ILogger<CommandHandler> logger, IOrganisationService serivce) : IRequestHandler<Command, CommandResponse>
{
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        // TODO: use high performance logging using LoggerMessage
        // if you have package referenced that supports high performance logging
        // replace with those methods.
        logger.LogInformation("Excecuting Command");

        return await serivce.ExecuteCreateCommand(request.Request);
    }
}