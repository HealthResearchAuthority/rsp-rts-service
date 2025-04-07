namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirExtension
{
    [JsonPropertyName("url")] public string Url { get; set; }

    [JsonPropertyName("valueBoolean")] public bool? ValueBoolean { get; set; }

    [JsonPropertyName("valueDate")] public string ValueDate { get; set; }

    [JsonPropertyName("valueString")] public string ValueString { get; set; }

    [JsonPropertyName("valueReference")] public RtsFhirValueReference ValueReference { get; set; }

    [JsonPropertyName("extension")] public List<RtsFhirSubExtension> Extension { get; set; }
}