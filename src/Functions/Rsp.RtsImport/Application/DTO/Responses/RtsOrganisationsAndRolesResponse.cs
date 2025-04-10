using System.Text.Json.Serialization;
using Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

namespace Rsp.RtsImport.Application.DTO.Responses;

public class RtsOrganisationsAndRolesResponse
{
    [JsonPropertyName("resourceType")] 
    public string ResourceType { get; set; }

    [JsonPropertyName("id")] 
    public string Id { get; set; }

    [JsonPropertyName("meta")] 
    public RtsFhirMeta Meta { get; set; }

    [JsonPropertyName("type")] 
    public string Type { get; set; }

    [JsonPropertyName("total")] 
    public int Total { get; set; }

    [JsonPropertyName("link")] 
    public List<RtsFhirLink> Link { get; set; }

    [JsonPropertyName("entry")] 
    public IEnumerable<RtsFhirEntry> Entry { get; set; }
}