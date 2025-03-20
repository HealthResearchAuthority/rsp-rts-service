using Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

namespace Rsp.RtsImport.Application.DTO.Responses;

public class RtsOrganisationsAndRolesResponse
{
    public string resourceType { get; set; }
    public string id { get; set; }
    public Meta meta { get; set; }
    public string type { get; set; }
    public int total { get; set; }
    public List<Link> link { get; set; }
    public IEnumerable<Entry> entry { get; set; }
}