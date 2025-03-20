namespace Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;

public class Address
{
    public string type { get; set; }
    public string text { get; set; }
    public IEnumerable<string> line { get; set; }
    public string city { get; set; }
    public string district { get; set; }
    public string postalCode { get; set; }
    public string country { get; set; }
}