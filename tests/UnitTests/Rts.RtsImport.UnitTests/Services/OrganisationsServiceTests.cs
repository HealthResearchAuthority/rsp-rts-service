using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Refit;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Application.DTO;
using Rsp.RtsImport.Application.DTO.Responses;
using Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;
using Rsp.RtsImport.Application.ServiceClients;
using Rsp.RtsImport.Application.Settings;
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
    public async Task FetchPageCountAsync_WhenValidDateString_FormatsAndReturnsTotal()
    {
        // Arrange
        const int expectedTotal = 42;

        var content = new RtsOrganisationsAndRolesResponse { Total = expectedTotal };
        var mockResponse = ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(content);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.Is<string>(s => s.StartsWith("gt", StringComparison.OrdinalIgnoreCase)), 0, 1))
            .ReturnsAsync(mockResponse);

        var sut = CreateSut();

        // Act
        var result = await sut.FetchPageCountAsync("2024-01-01");

        // Assert
        result.ShouldBe(expectedTotal);
        mockRtsClient.Verify(
            x => x.GetOrganisationsAndRoles(
                It.Is<string>(s => s.StartsWith("gt", StringComparison.OrdinalIgnoreCase) && s.EndsWith("Z", StringComparison.OrdinalIgnoreCase)),
                0,
                1),
            Times.Once);

        mockRtsClient.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task FetchPageCountAsync_WhenNullOrEmptyOrWhitespace_DoesNotSendLastUpdatedAndReturnsTotal(string? lastUpdated)
    {
        // Arrange
        const int expectedTotal = 10;

        var content = new RtsOrganisationsAndRolesResponse { Total = expectedTotal };
        var mockResponse = ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(content);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(null, 0, 1))
            .ReturnsAsync(mockResponse);

        var sut = CreateSut();

        // Act
        var result = await sut.FetchPageCountAsync(lastUpdated);

        // Assert
        result.ShouldBe(expectedTotal);
        mockRtsClient.Verify(x => x.GetOrganisationsAndRoles(null, 0, 1), Times.Once);
        mockRtsClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FetchPageCountAsync_WhenAlreadyGtValue_NormalisesAndReturnsTotal()
    {
        // Arrange
        const int expectedTotal = 7;

        var content = new RtsOrganisationsAndRolesResponse { Total = expectedTotal };
        var mockResponse = ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(content);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1))
            .ReturnsAsync(mockResponse);

        var sut = CreateSut();

        // Act
        var result = await sut.FetchPageCountAsync("gt2026-02-02T00:00:00Z");

        // Assert
        result.ShouldBe(expectedTotal);

        mockRtsClient.Verify(
            x => x.GetOrganisationsAndRoles(
                It.Is<string>(s =>
                    s.StartsWith("gt", StringComparison.OrdinalIgnoreCase) &&
                    s.EndsWith("Z", StringComparison.OrdinalIgnoreCase) &&
                    s.Contains(".") // will be .fff in your formatter
                ),
                0,
                1),
            Times.Once);

        mockRtsClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FetchPageCountAsync_WhenInvalidDate_ThrowsAndDoesNotCallClient()
    {
        // Arrange
        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        var sut = CreateSut();

        // Act
        var ex = await Should.ThrowAsync<ArgumentException>(() => sut.FetchPageCountAsync("not-a-date"));

        // Assert
        ex.Message.ShouldContain("Invalid");
        mockRtsClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FetchPageCountAsync_WhenApiResponseIsNull_ReturnsMinusOne()
    {
        // Arrange
        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1))
            .ReturnsAsync((ApiResponse<RtsOrganisationsAndRolesResponse>)null!);

        var sut = CreateSut();

        // Act
        var result = await sut.FetchPageCountAsync("2024-01-01");

        // Assert
        result.ShouldBe(-1);
        mockRtsClient.Verify(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1), Times.Once);
        mockRtsClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FetchPageCountAsync_WhenContentIsNull_ReturnsMinusOne()
    {
        // Arrange
        var mockResponse = ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(content: null);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1))
            .ReturnsAsync(mockResponse);

        var sut = CreateSut();

        // Act
        var result = await sut.FetchPageCountAsync("2024-01-01");

        // Assert
        result.ShouldBe(-1);
        mockRtsClient.Verify(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1), Times.Once);
        mockRtsClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FetchPageCountAsync_WhenOffsetAndCountAreFixed_AlwaysCallsWithZeroAndOne()
    {
        // Arrange
        var content = new RtsOrganisationsAndRolesResponse { Total = 1 };
        var mockResponse = ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(content);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(mockResponse);

        var sut = CreateSut();

        // Act
        await sut.FetchPageCountAsync("2024-01-01");

        // Assert
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

    [Fact]
    public async Task FetchPageCountAsync_WhenClientReturnsNull_ReturnsMinusOne()
    {
        // Arrange
        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1))
            .ReturnsAsync((ApiResponse<RtsOrganisationsAndRolesResponse>?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.FetchPageCountAsync("2024-01-01");

        // Assert
        result.ShouldBe(-1);
    }

    [Fact]
    public async Task FetchOrganisationAndRolesAsync_WhenSuccessful_ReturnsTransformedData()
    {
        // Arrange
        var entry = BuildValidFhirEntry(
            orgId: "ORG-001",
            oid: "ORG-ID-001",
            roleId: "ROLE-001",
            roleStatus: "Active",
            roleName: "ROLE-001_NAME",
            scoper: 123);

        var response = new RtsOrganisationsAndRolesResponse
        {
            Entry = new List<RtsFhirEntry> { entry }
        };

        var apiResponse = ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(response);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 100))
            .ReturnsAsync(apiResponse);

        var sut = CreateSut();

        // Act
        var result = (await sut.FetchOrganisationAndRolesAsync("2024-01-01", 0, 100)).ToList();

        // Assert
        result.ShouldNotBeEmpty();

        result[0].RtsOrganisation.ShouldNotBeNull();
        result[0].RtsOrganisation.Id.ShouldBe("ORG-001");
        result[0].RtsOrganisation.OId.ShouldBe("ORG-ID-001");

        result[0].RtsRole.Count.ShouldBe(1);
        var role = result[0].RtsRole.First();

        role.Id.ShouldBe("ROLE-001");
        role.Status.ShouldBe("Active");
        role.RoleName.ShouldBe("ROLE-001_NAME");
        role.Scoper.ShouldBe(123);
        role.OrganisationId.ShouldBe("ORG-001");
    }

    [Fact]
    public async Task FetchOrganisationAndRolesAsync_WhenUnsuccessful_ReturnsEmpty()
    {
        // Arrange
        var invalid = ApiResponseHelpers.CreateRtsOrganisationsAndRolesInvalidResponse(
            new RtsOrganisationsAndRolesResponse());

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 100))
            .ReturnsAsync(invalid);

        var sut = CreateSut();

        // Act
        var result = (await sut.FetchOrganisationAndRolesAsync("2024-01-01", 0, 100)).ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task FetchOrganisationAndRolesAsync_WhenClientThrows_ReturnsEmpty()
    {
        // Arrange
        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 100))
            .ThrowsAsync(new Exception("boom"));

        var sut = CreateSut();

        // Act
        var result = (await sut.FetchOrganisationAndRolesAsync("2024-01-01", 0, 100)).ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void TransformOrganisationAndRoles_WhenDuplicateRoleExtensions_OnlyAddsRoleOnce()
    {
        // Arrange
        var sut = CreateSut();

        var roleExt = BuildRoleExtension(
            roleId: "ROLE-001",
            roleStatus: "Active",
            roleName: "ROLE-001_NAME",
            scoper: 123,
            startDate: DateTime.UtcNow.ToString("yyyy-MM-dd"));

        var entry = BuildValidFhirEntry(
            orgId: "ORG-001",
            oid: "ORG-ID-001",
            roleId: "ROLE-001",
            roleStatus: "Active",
            roleName: "ROLE-001_NAME",
            scoper: 123);

        // Force duplicates
        entry.Resource.Extension = new List<RtsFhirExtension> { roleExt, roleExt };

        // Act
        var result = sut.TransformOrganisationAndRoles(entry);

        // Assert
        result.RtsOrganisation.ShouldNotBeNull();
        result.RtsRole.ShouldNotBeNull();
        result.RtsRole.Count.ShouldBe(1);
    }

    [Fact]
    public void TransformOrganisationAndRoles_WhenEntryInvalid_ReturnsEmptyContainer()
    {
        // Arrange
        var sut = CreateSut();

        var entry = new RtsFhirEntry
        {
            Resource = new RtsFhirOrganization
            {
                Id = "ORG-001",
                Identifier = new List<RtsFhirIdentifier>(), // Identifier[0] => throws
                Meta = new RtsFhirMeta { LastUpdated = DateTime.UtcNow },
                Active = true,
                Name = "Test Org",
                Address = new List<RtsFhirAddress>(),
                Type = new List<RtsFhirType>(),
                Extension = new List<RtsFhirExtension>()
            }
        };

        // Act
        var result = sut.TransformOrganisationAndRoles(entry);

        // Assert
        result.ShouldNotBeNull();
        result.RtsOrganisation.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateSponsorOrganisations_WhenOrganisationDisabled_DisablesAndAudits()
    {
        // Arrange
        const string orgId = "87795";

        var incoming = new Organisation
        {
            Id = orgId,
            Name = "Incoming Org",
            Status = false, // disabled org
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
                new() { Id = OrganisationRoles.Sponsor, Status = "active" }
            }
        };

        Mocker.GetMock<IOrganisationRepository>()
            .Setup(r => r.GetById(It.IsAny<OrganisationSpecification>()))
            .ReturnsAsync(existing);

        var sponsorOrgService = Mocker.GetMock<ISponsorOrganisationService>();
        sponsorOrgService.Setup(s => s.DisableSponsorOrganisation(orgId)).Returns(Task.CompletedTask);

        var auditService = Mocker.GetMock<IAuditService>();
        auditService.Setup(a => a.DatabaseSponsorOrganisationDisabled(existing.Name!)).Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        var count = await sut.UpdateSponsorOrganisations(new[] { incoming });

        // Assert
        count.ShouldBe(1);

        sponsorOrgService.Verify(s => s.DisableSponsorOrganisation(orgId), Times.Once);
        sponsorOrgService.Verify(s => s.EnableSponsorOrganisation(It.IsAny<string>()), Times.Never);

        auditService.Verify(a => a.DatabaseSponsorOrganisationDisabled(existing.Name!), Times.Once);
        auditService.Verify(a => a.DatabaseSponsorOrganisationEnabled(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateSponsorOrganisations_WhenStatusChangesToTerminated_CallsDisableAndAudits()
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
                new() { Id = OrganisationRoles.Sponsor, Status = "terminated" }
            }
        };

        var existing = new Organisation
        {
            Id = orgId,
            Name = "Existing Org",
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
        sponsorOrgService.Setup(s => s.DisableSponsorOrganisation(orgId)).Returns(Task.CompletedTask);

        var auditService = Mocker.GetMock<IAuditService>();
        auditService.Setup(a => a.DatabaseSponsorOrganisationDisabled(existing.Name!)).Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        await sut.UpdateSponsorOrganisations(new[] { incoming });

        // Assert
        sponsorOrgService.Verify(s => s.DisableSponsorOrganisation(orgId), Times.Once);
        sponsorOrgService.Verify(s => s.EnableSponsorOrganisation(It.IsAny<string>()), Times.Never);

        auditService.Verify(a => a.DatabaseSponsorOrganisationDisabled(existing.Name!), Times.Once);
        auditService.Verify(a => a.DatabaseSponsorOrganisationEnabled(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateSponsorOrganisations_WhenRoleStatusUnknown_DoesNothing()
    {
        // Arrange
        const string orgId = "87795";

        var incoming = new Organisation
        {
            Id = orgId,
            Status = true,
            Roles = new List<OrganisationRole>
            {
                new() { Id = OrganisationRoles.Sponsor, Status = "pending" } // unknown
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
        var auditService = Mocker.GetMock<IAuditService>();

        var sut = CreateSut();

        // Act
        await sut.UpdateSponsorOrganisations(new[] { incoming });

        // Assert
        sponsorOrgService.Verify(s => s.EnableSponsorOrganisation(It.IsAny<string>()), Times.Never);
        sponsorOrgService.Verify(s => s.DisableSponsorOrganisation(It.IsAny<string>()), Times.Never);
        sponsorOrgService.VerifyNoOtherCalls();

        auditService.Verify(a => a.DatabaseSponsorOrganisationEnabled(It.IsAny<string>()), Times.Never);
        auditService.Verify(a => a.DatabaseSponsorOrganisationDisabled(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetOrganisationsAndRoles_WhenTotalIsZero_ReturnsEmpty()
    {
        // Arrange
        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();

        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1))
            .ReturnsAsync(MakeBundleResponse(0));

        var sut = CreateSut();

        // Act
        var result = await sut.GetOrganisationsAndRoles("2024-01-01");

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetOrganisationsAndRoles_WhenMultiplePages_ReturnsAllResults()
    {
        // Arrange
        var settings = Mocker.Get<AppSettings>();
        settings.ApiRequestPageSize = 2;

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();

        // First call = total count
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1))
            .ReturnsAsync(MakeBundleResponse(5));

        // Page 1
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 2))
            .ReturnsAsync(MakeBundleResponse(5));

        // Page 2
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 2, 2))
            .ReturnsAsync(MakeBundleResponse(5));

        // Page 3
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 4, 2))
            .ReturnsAsync(MakeBundleResponse(5));

        var sut = CreateSut();

        // Act
        var result = await sut.GetOrganisationsAndRoles("2024-01-01");

        // Assert
        mockRtsClient.Verify(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 2), Times.Once);
        mockRtsClient.Verify(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 2, 2), Times.Once);
        mockRtsClient.Verify(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 4, 2), Times.Once);
    }

    [Fact]
    public async Task GetOrganisationsAndRoles_WhenLastUpdatedIsNull_PassesNullToClient()
    {
        // Arrange
        var settings = Mocker.Get<AppSettings>();
        settings.ApiRequestPageSize = 100;

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();

        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(null, 0, 1))
            .ReturnsAsync(MakeBundleResponse(1));

        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(null, 0, 100))
            .ReturnsAsync(MakeBundleResponse(1));

        var sut = CreateSut();

        // Act
        await sut.GetOrganisationsAndRoles(null);

        // Assert
        mockRtsClient.Verify(x => x.GetOrganisationsAndRoles(null, 0, 1), Times.Once);
        mockRtsClient.Verify(x => x.GetOrganisationsAndRoles(null, 0, 100), Times.Once);
    }

    [Fact]
    public async Task GetOrganisationsAndRoles_WhenDateProvided_FormatsGtParameter()
    {
        // Arrange
        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();

        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.Is<string>(s =>
                s.StartsWith("gt") && s.EndsWith("Z")), 0, 1))
            .ReturnsAsync(MakeBundleResponse(0));

        var sut = CreateSut();

        // Act
        await sut.GetOrganisationsAndRoles("2026-02-02");

        // Assert
        mockRtsClient.Verify(x =>
                x.GetOrganisationsAndRoles(
                    It.Is<string>(s => s.StartsWith("gt") && s.EndsWith("Z")),
                    0,
                    1),
            Times.Once);
    }

    private static RtsFhirExtension BuildRoleExtension(
        string roleId,
        string roleStatus,
        string roleName,
        int scoper,
        string startDate)
    {
        return new RtsFhirExtension
        {
            Extension = new List<RtsFhirSubExtension>
            {
                new() { Url = "startDate", ValueDate = startDate },
                new() { Url = "status", ValueString = roleStatus },
                new() { Url = "identifier", ValueString = roleId },
                new() { Url = "name", ValueString = roleName },
                new()
                {
                    Url = "scoper",
                    ValueReference = new RtsFhirValueReference { Reference = $"Organisation/{scoper}" }
                }
            }
        };
    }

    private static RtsFhirEntry BuildValidFhirEntry(
        string orgId,
        string oid,
        string roleId,
        string roleStatus,
        string roleName,
        int scoper)
    {
        return new RtsFhirEntry
        {
            Resource = new RtsFhirOrganization
            {
                Id = orgId,
                Identifier = new List<RtsFhirIdentifier> { new() { Value = oid } },
                Meta = new RtsFhirMeta { LastUpdated = DateTime.UtcNow },
                Active = true,
                Name = "Test Organisation",
                Address = new List<RtsFhirAddress> { new() { Text = "123 Fake Street", Country = "UK" } },
                Type = new List<RtsFhirType>
                {
                    new()
                    {
                        Text = "Gov",
                        Coding = new List<RtsFhirCoding> { new() { Code = "GOV001" } }
                    }
                },
                Extension = new List<RtsFhirExtension>
                {
                    BuildRoleExtension(
                        roleId: roleId,
                        roleStatus: roleStatus,
                        roleName: roleName,
                        scoper: scoper,
                        startDate: DateTime.UtcNow.ToString("yyyy-MM-dd"))
                }
            }
        };
    }

    private static ApiResponse<RtsOrganisationsAndRolesResponse> MakeBundleResponse(
        int total,
        params RtsFhirEntry[] entries)
    {
        var bundle = new RtsOrganisationsAndRolesResponse
        {
            ResourceType = "Bundle",
            Id = Guid.NewGuid().ToString(),
            Type = "searchset",
            Total = total,
            Meta = new RtsFhirMeta(),
            Entry = entries
        };

        return ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(bundle);
    }
}