using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirOrganization
{
    [JsonPropertyName("resourceType")]
    public string ResourceType { get; set; } = null!;

    [JsonPropertyName("id")] 
    public string Id { get; set; } = null!;

    [JsonPropertyName("meta")] 
    public RtsFhirMeta Meta { get; set; } = null!;

    [JsonPropertyName("extension")]
    public List<RtsFhirExtension> Extension { get; set; } = [];

    [JsonPropertyName("identifier")]
    public List<RtsFhirIdentifier> Identifier { get; set; } = [];

    [JsonPropertyName("active")] 
    public bool Active { get; set; }

    [JsonPropertyName("type")]
    public List<RtsFhirType> Type { get; set; } = [];

    [JsonPropertyName("name")] 
    public string Name { get; set; } = null!;

    [JsonPropertyName("address")]
    public List<RtsFhirAddress> Address { get; set; } = [];
}