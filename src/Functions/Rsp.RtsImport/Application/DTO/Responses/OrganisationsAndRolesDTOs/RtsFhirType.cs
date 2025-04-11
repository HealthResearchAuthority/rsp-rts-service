using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirType
{
    [JsonPropertyName("coding")] 
    public List<RtsFhirCoding> Coding { get; set; } = [];
    [JsonPropertyName("text")] 
    public string Text { get; set; } = null!;
}