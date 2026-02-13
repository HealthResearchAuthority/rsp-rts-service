using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsImport.Application.Contracts;

public interface ISponsorOrganisationService
{
    Task DisableSponsorOrganisation(string rtsId);

    Task EnableSponsorOrganisation(string rtsId);
}