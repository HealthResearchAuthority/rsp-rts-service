using Mapster;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.Contracts.Services;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Application.Enums;
using Rsp.RtsService.Application.Specifications;

namespace Rsp.RtsService.Services;

public class OrganisationService(IOrganisationRepository repository) : IOrganisationService
{
    public async Task<GetOrganisationByIdDto> GetById(string id)
    {
        var record = await repository.GetById(new OrganisationSpecification(id));
        return record.Adapt<GetOrganisationByIdDto>();
    }

    public async Task<IEnumerable<SearchOrganisationByNameDto>> SearchByName(string name, int pageSize, string? role = null, SortOrder sortOrder = SortOrder.Ascending)
    {
        var records = await repository.SearchByName(new OrganisationSpecification(name, pageSize, role!, sortOrder));

        return records.Adapt<IEnumerable<SearchOrganisationByNameDto>>();
    }
}