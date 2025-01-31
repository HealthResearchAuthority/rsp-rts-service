using Mapster;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.Contracts.Services;
using Rsp.RtsService.Application.Specifications;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Services;
public class OrganisationService(IOrganisationRepository repository) : IOrganisationService
{
    public async Task<Organisation> GetById(string id)
    {
        var record = await repository.GetById(new OrganisationSpecification(id));
        return record;
    }

    public async Task<IEnumerable<OrganisationSearchResult>> SearchByName(string name, string? type = null)
    {
        var result = await repository
            .SearchByName(new OrganisationSpecification(name, type));

        return result;
    }
}