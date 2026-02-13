using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Application.ServiceClients;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.DTOS;
using Rsp.RtsService.Application.Specifications;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsImport.Services;

public class SponsorOrganisationService(ISponsorOrganisationsServiceClient client)
    : ISponsorOrganisationService
{
    public async Task DisableSponsorOrganisation(string rtsId)
    {
        await client.DisableSponsorOrganisation(rtsId);
    }

    public async Task EnableSponsorOrganisation(string rtsId)
    {
        await client.EnableSponsorOrganisation(rtsId);
    }
}