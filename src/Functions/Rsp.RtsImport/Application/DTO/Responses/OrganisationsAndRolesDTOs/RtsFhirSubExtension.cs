namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirSubExtension
{
    [JsonPropertyName("url")] public string Url { get; set; }

    [JsonPropertyName("valueString")] public string ValueString { get; set; }

    [JsonPropertyName("valueDate")] public string ValueDate { get; set; }

    [JsonPropertyName("valueBoolean")] public bool? ValueBoolean { get; set; }

    [JsonPropertyName("valueReference")] public RtsFhirValueReference ValueReference { get; set; }
}