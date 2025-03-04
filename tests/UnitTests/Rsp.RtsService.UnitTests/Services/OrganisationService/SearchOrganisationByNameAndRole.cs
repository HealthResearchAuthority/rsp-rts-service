using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.EntityFrameworkCore;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;
using Rsp.RtsService.Infrastructure.Repositories;
using Rsp.RtsService.Services;
using Shouldly;

namespace Rsp.RtsService.UnitTests.Services.OrganisationRoleServiceTest;

public class SearchOrganisationByNameAndRole : TestServiceBase
{
    private readonly OrganisationRepository _rolesRepository;
    private readonly RtsDbContext _context;

    public SearchOrganisationByNameAndRole()
    {
        var options = new DbContextOptionsBuilder<RtsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options;

        _context = new RtsDbContext(options);
        _rolesRepository = new OrganisationRepository(_context);
    }

    [Theory]
    [InlineAutoData(5)]
    public async Task Returns_Organisations_For_Name(int records, Generator<Organisation> generator)
    {
        Mocker.Use<IOrganisationRepository>(_rolesRepository);

        var service = Mocker.CreateInstance<OrganisationService>();
        var testOrganisations = await TestData.SeedData(_context, generator, records);

        var nameToTest = "life";

        // expectedResults
        var organisations = testOrganisations
            .Where(x => x.Name.Contains(nameToTest));
        var organisationIds = organisations.Select(x => x.Id).ToList();

        // actualResults
        var actualOrganisations = await service.SearchByName(nameToTest, 10, null);
        var actualOrganisationIds = actualOrganisations.Select(x => x.Id).ToList();

        actualOrganisationIds.ShouldNotBeNull();
        actualOrganisationIds.ShouldBeEquivalentTo(organisationIds); // check results IDs
    }
}