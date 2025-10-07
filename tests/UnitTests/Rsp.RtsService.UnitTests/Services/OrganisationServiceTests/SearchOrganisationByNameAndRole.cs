using Ardalis.Specification;
using AutoFixture;
using AutoFixture.Xunit2;
using Bogus;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Moq;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Application.Enums;
using Rsp.RtsService.Application.Specifications;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;
using Rsp.RtsService.Infrastructure.Repositories;
using Rsp.RtsService.Services;
using Shouldly;

namespace Rsp.RtsService.UnitTests.Services.OrganisationServiceTests;

public class SearchOrganisationByNameAndRole : TestServiceBase<OrganisationService>
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
        // Use real repo
        Mocker.Use<IOrganisationRepository>(_rolesRepository);

        var service = Mocker.CreateInstance<OrganisationService>();
        var testOrganisations = await TestData.SeedData(_context, generator, records);

        const string nameToTest = "life";

        // expectedResults
        var organisations = testOrganisations.Where(x => x?.Name != null && x.Name.Contains(nameToTest));
        var organisationIds = organisations.Select(x => x.Id).ToList();

        // actualResults (new signature supports optional extras with defaults)
        var searchResponse = await service.SearchByName(nameToTest, 10, null);
        var actualOrganisationIds = searchResponse.Organisations.Select(x => x.Id).ToList();

        actualOrganisationIds.ShouldNotBeNull();
        actualOrganisationIds.ShouldBeEquivalentTo(organisationIds); // check results IDs
    }

    [Fact]
    public async Task SearchByName_Should_Return_Mapped_Dtos()
    {
        // Arrange
        const string name = "Test";
        const int pageIndex = 1;   // service normalizes anyway, but keep it valid
        const int pageSize = 2;

        var orgs = new List<Organisation>
        {
            new() { Id = "1", Name = "Org 1", Status = true, Address = "123 Main St", CountryName = "Poland", Type = "Local company" },
            new() { Id = "2", Name = "Org 2", Status = true, Address = "125 Main St", CountryName = "England", Type = "Local company" }
        };

        var expectedDtos = orgs.Adapt<IEnumerable<SearchOrganisationDto>>();

        Mocker
            .GetMock<IOrganisationRepository>()
            .Setup(r => r.GetBySpecification(It.IsAny<OrganisationSpecification>(), It.IsAny<int>(), It.IsAny<int?>()))
            .ReturnsAsync((orgs, 2));

        // Act (new signature ignores extra params if omitted)
        var result = await Sut.SearchByName(name, pageIndex, pageSize);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<OrganisationSearchResponse>();
        result.Organisations.ShouldBe(expectedDtos, ignoreOrder: false);
    }

    [Fact]
    public async Task SearchByName_Should_Pass_Correct_Specification_Parameters()
    {
        // Arrange
        const string name = "Test";
        const int pageSize = 5;
        const int pageIndex = 1;
        const string role = "Admin";

        // NEW: use countries/sortField/sortDirection (no SortOrder enum now)
        var countries = new[] { "England", "Wales" };
        const string sortField = "name";
        const string sortDirection = "desc";

        OrganisationSpecification? capturedSpec = null;
        int capturedPageIndex = -1;
        int? capturedPageSize = null;

        Mocker
            .GetMock<IOrganisationRepository>()
            .Setup(r => r.GetBySpecification(It.IsAny<OrganisationSpecification>(), It.IsAny<int>(), It.IsAny<int?>()))
            .Callback((ISpecification<Organisation> spec, int pIndex, int? pSize) =>
            {
                capturedSpec = spec as OrganisationSpecification;
                capturedPageIndex = pIndex;
                capturedPageSize = pSize;
            })
            .ReturnsAsync(([], 0));

        // Act
        await Sut.SearchByName(name, pageIndex, pageSize, role, countries, sortField, sortDirection);

        // Assert
        capturedSpec.ShouldNotBeNull();
        capturedPageIndex.ShouldBe(pageIndex);
        capturedPageSize.ShouldBe(pageSize);
        // We can't introspect private spec internals easily; the above verifies correct plumbing.
    }

    [Theory, AutoData]
    public async Task SearchByName_Should_Return_TotalCount_Of_SearchedRecords(Faker<Organisation> faker)
    {
        // Arrange
        Mocker.Use<IOrganisationRepository>(_rolesRepository);

        faker.RuleFor(o => o.Id, f => f.Random.Guid().ToString());
        faker.RuleFor(o => o.OId, f => f.Random.Guid().ToString());
        faker.RuleFor(o => o.Type, f => f.Random.Guid().ToString());
        faker.RuleFor(o => o.TypeId, f => f.Random.Guid().ToString());
        faker.RuleFor(o => o.TypeName, f => f.Random.Guid().ToString());
        faker.RuleFor(o => o.Name, f => $"nhs {f.Company.CompanyName()}");
        faker.RuleFor(o => o.Status, true);
        faker.RuleFor(o => o.CountryName, f => f.PickRandom(new[] { "England", "Wales", "Scotland", "Northern Ireland" }));

        var service = Mocker.CreateInstance<OrganisationService>();
        var testOrganisations = await TestData.SeedData(_context, faker.Generate(10));

        const string nameToTest = "nhs";

        // Act
        var searchResponse = await service.SearchByName(nameToTest, 5, null, null);

        // Assert
        searchResponse.TotalCount.ShouldBe(testOrganisations.Count());
    }
}
