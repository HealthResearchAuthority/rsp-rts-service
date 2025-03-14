﻿using Rsp.RtsService.Application.Contracts.Repositories;
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

    public async Task<IEnumerable<Organisation>> SearchByName(string name, int pageSize, string? role = null)
    {
        var records = await repository.SearchByName(new OrganisationSpecification(name, pageSize, role!));

        return records;
    }
}