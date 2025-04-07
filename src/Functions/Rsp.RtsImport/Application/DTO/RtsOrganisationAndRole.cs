namespace Rsp.RtsImport.Application.DTO;

public class RtsOrganisationAndRole
{
    public Organisation rtsOrganisation { get; set; }
    public IList<OrganisationRole> rtsRole { get; set; }
}