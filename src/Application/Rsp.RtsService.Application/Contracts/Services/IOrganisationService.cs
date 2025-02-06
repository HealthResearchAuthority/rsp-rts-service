using Rsp.RtsService.Application.DTOS.Requests;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Contracts.Services;

/// <summary>
/// Interface to create/read/update the records in the database
/// </summary>
public interface IOrganisationService
{
    /// <summary>
    /// Search organisations by their full or partial name
    /// </summary>
    Task<IEnumerable<OrganisationSearchResult>> SearchByName(string name, string? type = null);

    /// <summary>
    /// Get organisation by it's id
    /// </summary>
    Task<Organisation> GetById(string id);    
}