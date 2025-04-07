namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirAddress
{
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("text")] public string Text { get; set; }

    [JsonPropertyName("line")] public List<string> Line { get; set; }

    [JsonPropertyName("city")] public string City { get; set; }

    [JsonPropertyName("district")] public string District { get; set; }

    [JsonPropertyName("postalCode")] public string PostalCode { get; set; }

    [JsonPropertyName("country")] public string Country { get; set; }
}