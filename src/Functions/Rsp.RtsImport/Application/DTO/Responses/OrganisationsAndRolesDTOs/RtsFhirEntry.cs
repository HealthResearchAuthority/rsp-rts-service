namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class RtsFhirEntry
{
    [JsonPropertyName("fullUrl")] public string FullUrl { get; set; }

    [JsonPropertyName("resource")] public RtsFhirOrganization Resource { get; set; }
}