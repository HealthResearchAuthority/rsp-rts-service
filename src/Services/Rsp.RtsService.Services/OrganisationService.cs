using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.Contracts.Services;
using Rsp.RtsService.Application.Specifications;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Services;

public class OrganisationService(IOrganisationRepository orgRepo, IOrganisationRoleRepository rolesRepo) : IOrganisationService
{
    public async Task<Organisation> GetById(string id)
    {
        var record = await orgRepo.GetById(new OrganisationSpecification(id));
        return record;
    }

    public async Task<IEnumerable<OrganisationSearchResult>> SearchByName(string name, string? type = null, string? role = null)
    {
        var result = await rolesRepo
            .SearchByName(new OrganisationRoleSpecification(name, type, role));

        return result;
    }
}