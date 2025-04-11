using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsImport.Application.DTO;

public class RtsOrganisationAndRole
{
    public Organisation RtsOrganisation { get; set; } = null!;
    public IList<OrganisationRole> RtsRole { get; set; } = [];
}