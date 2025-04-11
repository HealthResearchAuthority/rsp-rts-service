using System.Text.Json.Serialization;
using Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

namespace Rsp.RtsImport.Application.DTO.Responses;

public class RtsOrganisationsAndRolesResponse
{
    [JsonPropertyName("resourceType")] 
    public string ResourceType { get; set; } = null!;

    [JsonPropertyName("id")] 
    public string Id { get; set; } = null!;

    [JsonPropertyName("meta")] 
    public RtsFhirMeta Meta { get; set; } = null!;

    [JsonPropertyName("type")] 
    public string Type { get; set; } = null!;

    [JsonPropertyName("total")] 
    public int Total { get; set; }

    [JsonPropertyName("link")]
    public List<RtsFhirLink> Link { get; set; } = [];

    [JsonPropertyName("entry")]
    public IEnumerable<RtsFhirEntry> Entry { get; set; } = [];
}