using Rsp.RtsImport.Application.DTO;
using Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsImport.Application.Contracts;

public interface IMetadataService
{
    /// <summary>
    /// Get metadata
    /// </summary>
    Task<Metadata?> GetMetaData();

    /// <summary>
    /// Get metadata
    /// </summary>
    Task<Metadata?> UpdateLastUpdated();
}