using Rsp.RtsService.Application.DTOS.Requests;
using Rsp.RtsService.Application.DTOS.Responses;

namespace Rsp.RtsService.Application.Contracts.Services;

/// <summary>
/// Interface to create/read/update the records in the database
/// </summary>
public interface IRtsService
{
    /// <summary>
    /// Returns a single entity
    /// </summary>
    /// <param name="id">Id of the entity</param>
    Task<QueryResponse> ExecuteQuery(int id);

    /// <summary>
    /// Adds a new entity to the database
    /// </summary>
    /// <param name="request">The values</param>
    Task<CommandResponse> ExecuteCreateCommand(CommandRequest request);

    /// <summary>
    /// Updates an entity in the database
    /// </summary>
    /// <param name="id">Entity Id</param>
    /// <param name="request">Updated Entity Values</param>
    Task<CommandResponse> ExcecuteUpdateCommand(int id, CommandRequest request);
}