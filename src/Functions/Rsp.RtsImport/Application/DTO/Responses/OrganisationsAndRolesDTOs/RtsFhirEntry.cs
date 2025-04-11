using System.Text.Json.Serialization;

namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirEntry
{
    [JsonPropertyName("fullUrl")] 
    public string FullUrl { get; set; } = null!;

    [JsonPropertyName("resource")] 
    public RtsFhirOrganization Resource { get; set; } = null!;
}