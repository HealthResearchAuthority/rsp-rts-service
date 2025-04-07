namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirValueReference
{
    [JsonPropertyName("reference")] public string Reference { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("display")] public string Display { get; set; }
}