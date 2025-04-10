using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirValueReference
{
    [JsonPropertyName("reference")] 
    public string Reference { get; set; } = null!;

    [JsonPropertyName("type")] 
    public string Type { get; set; } = null!;

    [JsonPropertyName("display")] 
    public string Display { get; set; } = null!;
}