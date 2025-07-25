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

public class GetAllTests : TestServiceBase<OrganisationService>
{
    private readonly OrganisationRepository _rolesRepository;
    private readonly RtsDbContext _context;

    public GetAllTests()
    {
        var options = new DbContextOptionsBuilder<RtsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options;

        _context = new RtsDbContext(options);
        _rolesRepository = new OrganisationRepository(_context);
    }

    [Theory]
    [InlineAutoData(10)]
    public async Task Returns_All_Organisations(int records, Generator<Organisation> generator)
    {
        // Arrange
        int pageSize = 10;
        Mocker.Use<IOrganisationRepository>(_rolesRepository);

        var service = Mocker.CreateInstance<OrganisationService>();
        var testOrganisations = await TestData.SeedData(_context, generator, records);

        var expectedOrganisationIds = testOrganisations
            .Where(x => x != null)
            .OrderBy(x => x.Name)
            .Select(x => x.Id)
            .ToList();

        // Act
        var getResponse = await service.GetAll(pageSize, null);
        var actualOrganisationIds = getResponse.Organisations.Select(x => x.Id).ToList();

        // Assert
        actualOrganisationIds.ShouldNotBeNull();
        actualOrganisationIds.ShouldBeEquivalentTo(expectedOrganisationIds);
    }

    [Fact]
    public async Task GetAll_Should_Pass_Correct_Specification_Parameters()
    {
        // Arrange
        const string role = "Admin";
        const SortOrder sortOrder = SortOrder.Descending;
        const int pageSize = 10;

        OrganisationSpecification? capturedSpec = null;

        Mocker
            .GetMock<IOrganisationRepository>()
            .Setup(r => r.GetBySpecification(It.IsAny<OrganisationSpecification>(), It.IsAny<int>()))
            .Callback((ISpecification<Organisation> spec, int _) => capturedSpec = spec as OrganisationSpecification)
            .ReturnsAsync(([], 0));

        // Act
        await Sut.GetAll(pageSize, role, sortOrder);

        // Assert
        capturedSpec.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAll_Should_Return_Mapped_Dtos()
    {
        // Arrange
        int pageSize = 10;

        var orgs = new List<Organisation>
        {
            new() { Id = "1", Name = "Org 1", Status = true, Address = "123 Main St", CountryName = "Poland", Type = "Local company" },
            new() { Id = "2", Name = "Org 2", Status = true, Address = "125 Main St", CountryName = "England", Type = "Local company" }
        };

        var expectedDtos = orgs.Adapt<IEnumerable<SearchOrganisationDto>>();

        Mocker
            .GetMock<IOrganisationRepository>()
            .Setup(r => r.GetBySpecification(It.IsAny<OrganisationSpecification>(), It.IsAny<int>()))
            .ReturnsAsync((orgs, orgs.Count));

        // Act
        var result = await Sut.GetAll(pageSize);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<OrganisationSearchResponse>();
        result.Organisations.ShouldBe(expectedDtos, ignoreOrder: false);
    }

    [Theory, AutoData]
    public async Task GetAll_Should_Return_TotalCount_Of_Records(Faker<Organisation> faker)
    {
        // Arrange
        Mocker.Use<IOrganisationRepository>(_rolesRepository);

        faker.RuleFor(o => o.Id, f => f.Random.Guid().ToString());
        faker.RuleFor(o => o.Name, f => f.Company.CompanyName());
        faker.RuleFor(o => o.Status, true);
        faker.RuleFor(o => o.OId, f => f.Random.Guid().ToString());
        faker.RuleFor(o => o.Type, f => f.Commerce.Department());
        faker.RuleFor(o => o.TypeId, f => f.Random.Guid().ToString());
        faker.RuleFor(o => o.TypeName, f => f.Commerce.ProductName());

        var service = Mocker.CreateInstance<OrganisationService>();
        var testOrganisations = await TestData.SeedData(_context, faker.Generate(10));

        // Act
        var response = await service.GetAll(5);

        // Assert
        response.TotalCount.ShouldBe(testOrganisations.Count());
    }
}