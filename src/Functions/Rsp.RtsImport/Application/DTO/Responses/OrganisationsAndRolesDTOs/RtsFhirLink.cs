using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirLink
{
    [JsonPropertyName("relation")] 
    public string Relation { get; set; } = null!;
    [JsonPropertyName("url")] 
    public string Url { get; set; } = null!;
}