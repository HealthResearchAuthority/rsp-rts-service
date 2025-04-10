using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirCoding
{
    [JsonPropertyName("code")] 
    public string Code { get; set; } = null!;
}