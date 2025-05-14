using Ardalis.Specification;
using AutoFixture;
using AutoFixture.Xunit2;
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
        Mocker.Use<IOrganisationRepository>(_rolesRepository);

        var service = Mocker.CreateInstance<OrganisationService>();
        var testOrganisations = await TestData.SeedData(_context, generator, records);

        const string nameToTest = "life";

        // expectedResults
        var organisations = testOrganisations.Where(x => x?.Name != null && x.Name.Contains(nameToTest));

        var organisationIds = organisations.Select(x => x.Id).ToList();

        // actualResults
        var actualOrganisations = await service.SearchByName(nameToTest, 10, null);
        var actualOrganisationIds = actualOrganisations.Select(x => x.Id).ToList();

        actualOrganisationIds.ShouldNotBeNull();
        actualOrganisationIds.ShouldBeEquivalentTo(organisationIds); // check results IDs
    }

    [Fact]
    public async Task SearchByName_Should_Return_Mapped_Dtos()
    {
        // Arrange
        const string name = "Test";
        const int pageSize = 2;

        var orgs = new List<Organisation>
        {
            new() { Id = "1", Name = "Test Org 1" },
            new() { Id = "2", Name = "Test Org 2" }
        };

        var expectedDtos = orgs.Adapt<IEnumerable<SearchOrganisationByNameDto>>();

        Mocker
            .GetMock<IOrganisationRepository>()
            .Setup(r => r.SearchByName(It.IsAny<OrganisationSpecification>()))
            .ReturnsAsync(orgs);

        // Act
        var result = await Sut.SearchByName(name, pageSize);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IEnumerable<SearchOrganisationByNameDto>>();
        result.ShouldBe(expectedDtos, ignoreOrder: false);
    }

    [Fact]
    public async Task SearchByName_Should_Pass_Correct_Specification_Parameters()
    {
        // Arrange
        const string name = "Test";
        const int pageSize = 5;
        const string role = "Admin";
        const SortOrder sortOrder = SortOrder.Descending;

        var orgs = new List<Organisation>();

        OrganisationSpecification? capturedSpec = null;
        Mocker
            .GetMock<IOrganisationRepository>()
            .Setup(r => r.SearchByName(It.IsAny<OrganisationSpecification>()))
            .Callback<ISpecification<Organisation>>(spec => capturedSpec = spec as OrganisationSpecification)
            .ReturnsAsync(orgs);

        // Act
        await Sut.SearchByName(name, pageSize, role, sortOrder);

        // Assert
        capturedSpec.ShouldNotBeNull();
    }
}