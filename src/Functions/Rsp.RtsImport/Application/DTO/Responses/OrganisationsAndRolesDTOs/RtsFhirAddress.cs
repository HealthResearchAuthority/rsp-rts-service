using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirAddress
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("text")] 
    public string Text { get; set; } = null!;

    [JsonPropertyName("line")] 
    public List<string> Line { get; set; } = null!;

    [JsonPropertyName("city")] 
    public string City { get; set; } = null!;

    [JsonPropertyName("district")] 
    public string District { get; set; } = null!;

    [JsonPropertyName("postalCode")] 
    public string PostalCode { get; set; } = null!;

    [JsonPropertyName("country")] 
    public string Country { get; set; } = null!;
}