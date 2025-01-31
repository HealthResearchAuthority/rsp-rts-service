﻿using Ardalis.Specification;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsService.Application.Contracts.Repositories;

public interface IOrganisationRepository
{
    Task<IEnumerable<OrganisationSearchResult>> SearchByName(ISpecification<Organisation> specification);

    Task<Organisation> GetById(ISpecification<Organisation> specification);
}