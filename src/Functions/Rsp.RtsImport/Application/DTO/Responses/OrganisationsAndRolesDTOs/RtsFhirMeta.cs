using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirMeta
{
    [JsonPropertyName("lastUpdated")] 
    public DateTime LastUpdated { get; set; }
}