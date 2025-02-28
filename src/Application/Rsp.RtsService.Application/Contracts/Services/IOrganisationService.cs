using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Contracts.Services;

/// <summary>
/// Interface to create/read/update the records in the database
/// </summary>
public interface IOrganisationService
{
    /// <summary>
    /// Get organisation by it's id
    /// </summary>
    Task<Organisation> GetById(string id);

    Task<IEnumerable<Organisation>> SearchByName(string name, int pageSize, string? role = null);
}