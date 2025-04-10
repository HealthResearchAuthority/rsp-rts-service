using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirIdentifier
{
    [JsonPropertyName("value")] 
    public string Value { get; set; } = null!;
}