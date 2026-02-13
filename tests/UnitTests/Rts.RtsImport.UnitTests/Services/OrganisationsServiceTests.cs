using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Application.DTO.Responses;
using Rsp.RtsImport.Application.ServiceClients;
using Rsp.RtsImport.Services;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.Specifications;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;
using Rts.RtsImport.UnitTests.Helpers;
using Shouldly;

namespace Rts.RtsImport.UnitTests.Services;

public class OrganisationsServiceTests : TestServiceBase, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly RtsDbContext _context;

    public OrganisationsServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<RtsDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new RtsDbContext(options);
        _context.Database.EnsureCreated();

        // Ensure the SUT gets THIS context instance
        Mocker.Use(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }

    private OrganisationsService CreateSut()
    {
        return Mocker.CreateInstance<OrganisationsService>();
    }

    [Theory]
    [InlineAutoData(5)]
    public async Task UpdateOrganisations_CallsUpdateOrganisations(
        int records,
        Generator<Organisation> generator)
    {
        // Arrange
        var sut = CreateSut();
        var seeded = await TestData.SeedOrganisations(_context, generator, records);

        // Act
        var result = await sut.UpdateOrganisations(seeded, true);

        // Assert
        result.ShouldNotBeNull();

        var allOrgs = await _context.Organisation.ToListAsync();
        allOrgs.ShouldNotBeEmpty();
        allOrgs[0].Status.ShouldBe(RtsRecordStatusOptions.ActiveOrg);
    }

    [Theory]
    [InlineAutoData(5)]
    public async Task UpdateRoles_InsertsOrUpdatesFilteredRolesOnly(
        int records,
        Generator<Organisation> generator,
        Generator<OrganisationRole> generatorOrganisationRoles)
    {
        // Arrange
        var sut = CreateSut();
        await TestData.SeedOrganisations(_context, generator, records);

        var roles = await TestData.SeedOrganisationRoles(_context, generatorOrganisationRoles, records, true);

        // Act
        var result = await sut.UpdateRoles(roles, true);

        // Assert
        result.ShouldNotBeNull();

        var rolesInDb = await _context.OrganisationRole.ToListAsync();
        rolesInDb.Count.ShouldBe(20);
    }

    [Theory]
    [InlineAutoData(5)]
    public async Task UpdateRoles_NoRoles(
        int records,
        Generator<Organisation> generator,
        Generator<OrganisationRole> generatorOrganisationRoles)
    {
        // Arrange
        var sut = CreateSut();
        await TestData.SeedOrganisations(_context, generator, records);

        var roles = await TestData.SeedOrganisationRoles(_context, generatorOrganisationRoles, records);

        // Act
        var result = await sut.UpdateRoles(roles, true);

        // Assert
        result.ShouldNotBeNull();
        result.RecordsUpdated.ShouldBe(0);
    }

    [Fact]
    public async Task FetchPageCountAsync_ReturnsCorrectTotal()
    {
        // Arrange
        const int expectedTotal = 42;

        var content = new RtsOrganisationsAndRolesResponse { Total = expectedTotal };
        var mockResponse = ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(content);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1))
            .ReturnsAsync(mockResponse);

        var sut = CreateSut();

        // Act
        var result = await sut.FetchPageCountAsync("2024-01-01");

        // Assert
        result.ShouldBe(expectedTotal);
        mockRtsClient.Verify(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1), Times.Once);
        mockRtsClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateSponsorOrganisations_WhenStatusChangesToActive_CallsEnable()
    {
        // Arrange
        const string orgId = "87795";

        var incoming = new Organisation
        {
            Id = orgId,
            Name = "Incoming Org",
            Status = true,
            Roles = new List<OrganisationRole>
            {
                new() { Id = OrganisationRoles.Sponsor, Status = "active" }
            }
        };

        var existing = new Organisation
        {
            Id = orgId,
            Name = "Existing Org",
            Status = true,
            Roles = new List<OrganisationRole>
            {
                new() { Id = OrganisationRoles.Sponsor, Status = "terminated" }
            }
        };

        var orgRepo = Mocker.GetMock<IOrganisationRepository>();
        orgRepo
            .Setup(r => r.GetById(It.IsAny<OrganisationSpecification>()))
            .ReturnsAsync(existing);

        var sponsorOrgService = Mocker.GetMock<ISponsorOrganisationService>();
        sponsorOrgService
            .Setup(s => s.EnableSponsorOrganisation(orgId))
            .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        await sut.UpdateSponsorOrganisations(new[] { incoming });

        // Assert
        sponsorOrgService.Verify(s => s.EnableSponsorOrganisation(orgId), Times.Once);
        sponsorOrgService.Verify(s => s.DisableSponsorOrganisation(It.IsAny<string>()), Times.Never);
        sponsorOrgService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateSponsorOrganisations_WhenStatusHasNotChanged_DoesNothing()
    {
        // Arrange
        const string orgId = "87795";

        var incoming = new Organisation
        {
            Id = orgId,
            Status = true,
            Roles = new List<OrganisationRole>
            {
                new() { Id = OrganisationRoles.Sponsor, Status = "active" }
            }
        };

        var existing = new Organisation
        {
            Id = orgId,
            Status = true,
            Roles = new List<OrganisationRole>
            {
                new() { Id = OrganisationRoles.Sponsor, Status = "active" }
            }
        };

        Mocker.GetMock<IOrganisationRepository>()
            .Setup(r => r.GetById(It.IsAny<OrganisationSpecification>()))
            .ReturnsAsync(existing);

        var sponsorOrgService = Mocker.GetMock<ISponsorOrganisationService>();

        var sut = CreateSut();

        // Act
        await sut.UpdateSponsorOrganisations(new[] { incoming });

        // Assert
        sponsorOrgService.Verify(s => s.EnableSponsorOrganisation(It.IsAny<string>()), Times.Never);
        sponsorOrgService.Verify(s => s.DisableSponsorOrganisation(It.IsAny<string>()), Times.Never);
        sponsorOrgService.VerifyNoOtherCalls();
    }
}