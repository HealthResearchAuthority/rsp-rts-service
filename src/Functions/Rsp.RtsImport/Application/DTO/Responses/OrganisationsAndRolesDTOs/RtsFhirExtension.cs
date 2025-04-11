using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirExtension
{
    [JsonPropertyName("url")] 
    public string Url { get; set; } = null!;

    [JsonPropertyName("valueBoolean")] 
    public bool? ValueBoolean { get; set; } = null!;

    [JsonPropertyName("valueDate")] 
    public string ValueDate { get; set; } = null!;

    [JsonPropertyName("valueString")] 
    public string ValueString { get; set; } = null!;

    [JsonPropertyName("valueReference")] 
    public RtsFhirValueReference ValueReference { get; set; } = null!;

    [JsonPropertyName("extension")] 
    public List<RtsFhirSubExtension> Extension { get; set; } = [];
}