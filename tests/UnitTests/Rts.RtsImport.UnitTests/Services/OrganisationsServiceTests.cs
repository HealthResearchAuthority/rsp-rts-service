using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.DTO.Responses;
using Rsp.RtsImport.Application.DTO.Responses.OrganisationsAndRolesDTOs;
using Rsp.RtsImport.Application.ServiceClients;
using Rsp.RtsImport.Services;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;
using Rts.RtsImport.UnitTests.Helpers;
using Shouldly;

namespace Rts.RtsImport.UnitTests.Services;

public class OrganisationsServiceTests : TestServiceBase
{
    private readonly SqliteConnection _connection;
    private readonly RtsDbContext _context;
    private readonly OrganisationsService _service;

    public OrganisationsServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<RtsDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new RtsDbContext(options);
        _context.Database.EnsureCreated();

        Mocker.Use(_context);
        _service = Mocker.CreateInstance<OrganisationsService>();
    }

    [Theory]
    [InlineAutoData(5)]
    public async Task UpdateOrganisations_CallsUpdateOrganisations(int records, Generator<Organisation> generator)
    {
        // Arrange + Act
        var result =
            await _service.UpdateOrganisations(await TestData.SeedOrganisations(_context, generator, records), true);

        // Assert
        var allOrgs = await _context.Organisation.ToListAsync();
        result.ShouldNotBeNull();
        allOrgs[0].Status.ShouldBe(RtsRecordStatusOptions.ActiveOrg);
    }

    [Theory]
    [InlineAutoData(5)]
    public async Task UpdateRoles_InsertsOrUpdatesFilteredRolesOnly(int records, Generator<Organisation> generator,
        Generator<OrganisationRole> generatorOrganisationRoles)
    {
        // Arrange
        await TestData.SeedOrganisations(_context, generator, records);

        // Act
        var result =
            await _service.UpdateRoles(
                await TestData.SeedOrganisationRoles(_context, generatorOrganisationRoles, records, true), true);

        // Assert
        result.ShouldNotBeNull();

        var rolesInDb = await _context.OrganisationRole.ToListAsync();
        rolesInDb.Count.ShouldBe(20);
    }

    [Theory]
    [InlineAutoData(5)]
    public async Task UpdateRoles_NoRoles(int records, Generator<Organisation> generator,
        Generator<OrganisationRole> generatorOrganisationRoles)
    {
        // Arrange
        await TestData.SeedOrganisations(_context, generator, records);

        // Act
        var result =
            await _service.UpdateRoles(
                await TestData.SeedOrganisationRoles(_context, generatorOrganisationRoles, records), true);

        // Assert
        result.ShouldNotBeNull();
        result.RecordsUpdated.ShouldBe(0);
    }

    [Fact]
    public async Task FetchPageCountAsync_ReturnsCorrectTotal()
    {
        // Arrange
        var expectedTotal = 42;

        var content = new RtsOrganisationsAndRolesResponse
        {
            Total = expectedTotal
        };

        var mockResponse = ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(content);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1))
            .ReturnsAsync(mockResponse);

        // Act
        var result = await _service.FetchPageCountAsync("2024-01-01");

        // Assert
        result.ShouldBe(expectedTotal);
    }

    [Fact]
    public async Task FetchOrganisationAndRolesAsync_ReturnsTransformedOrganisationAndRole()
    {
        // Arrange
        var fhirEntry = new RtsFhirEntry
        {
            Resource = new RtsFhirOrganization
            {
                Id = "ORG-001",
                Identifier = [new RtsFhirIdentifier { Value = "ORG-ID-001" }],
                Meta = new RtsFhirMeta
                {
                    LastUpdated = DateTime.UtcNow
                },
                Active = true,
                Name = "Test Organisation",
                Address =
                [
                    new RtsFhirAddress
                    {
                        Text = "123 Fake Street",
                        Country = "Testland"
                    }
                ],
                Type =
                [
                    new RtsFhirType
                    {
                        Text = "Gov",
                        Coding = [new RtsFhirCoding { Code = "GOV001" }]
                    }
                ],
                Extension =
                [
                    new RtsFhirExtension
                    {
                        Extension =
                        [
                            new RtsFhirSubExtension { Url = "startDate", ValueDate = DateTime.UtcNow.ToString("yyyy-MM-dd") },
                            new RtsFhirSubExtension { Url = "status", ValueString = "Active" },
                            new RtsFhirSubExtension { Url = "identifier", ValueString = "ROLE-001" },
                            new RtsFhirSubExtension
                            {
                                Url = "scoper",
                                ValueReference = new RtsFhirValueReference { Reference = "Organisation/123" }
                            }
                        ]
                    }
                ]
            }
        };

        var response = new RtsOrganisationsAndRolesResponse
        {
            Entry = [fhirEntry]
        };

        var apiResponse = ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(response);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 100))
            .ReturnsAsync(apiResponse);

        // Act
        var result = (await _service.FetchOrganisationAndRolesAsync("2024-01-01", 0, 100)).ToList();

        // Assert
        result.ShouldNotBeEmpty();
        result[0].RtsOrganisation.Name.ShouldBe("Test Organisation");
        result[0].RtsOrganisation.Type.ShouldBe("Gov");
        result[0].RtsOrganisation.OId.ShouldBe("ORG-ID-001");

        var role = result[0].RtsRole.First();
        role.Id.ShouldBe("ROLE-001");
        role.Status.ShouldBe("Active");
        role.Scoper.ShouldBe(123);
        role.OrganisationId.ShouldBe("ORG-001");
    }

    [Fact]
    public async Task FetchOrganisationAndRolesAsync_ReturnsInvalidResponse()
    {
        // Arrange
        var fhirEntry = new RtsFhirEntry();
        var response = new RtsOrganisationsAndRolesResponse
        {
            Entry = [fhirEntry]
        };

        var apiResponse = ApiResponseHelpers.CreateRtsOrganisationsAndRolesInvalidResponse(response);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 100))
            .ReturnsAsync(apiResponse);

        // Act
        var result = (await _service.FetchOrganisationAndRolesAsync("2024-01-01", 0, 100)).ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task FetchOrganisationAndRolesAsync_ReturnsEmpty()
    {
        // Arrange
        var fhirEntry = new RtsFhirEntry();

        var response = new RtsOrganisationsAndRolesResponse
        {
            Entry = [fhirEntry]
        };

        ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(response);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();
        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 100))
            .ThrowsAsync(new Exception());

        // Act
        var result = (await _service.FetchOrganisationAndRolesAsync("2024-01-01", 0, 100)).ToList();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetOrganisationsAndRoles_ReturnsDistinctTransformedOrganisationAndRoles()
    {
        // Arrange
        var fhirEntry = new RtsFhirEntry
        {
            Resource = new RtsFhirOrganization
            {
                Id = "ORG-001",
                Identifier =
                [
                    new RtsFhirIdentifier { Value = "ORG-ID-001" }
                ],
                Meta = new RtsFhirMeta
                {
                    LastUpdated = DateTime.UtcNow
                },
                Active = true,
                Name = "Test Organisation",
                Address =
                [
                    new RtsFhirAddress
                    {
                        Text = "123 Fake Street",
                        Country = "Testland"
                    }
                ],
                Type =
                [
                    new RtsFhirType
                    {
                        Text = "Gov",
                        Coding = [new RtsFhirCoding { Code = "GOV001" }]
                    }
                ],
                Extension =
                [
                    new RtsFhirExtension
                    {
                        Extension =
                        [
                            new RtsFhirSubExtension { Url = "startDate", ValueDate = DateTime.UtcNow.ToString("yyyy-MM-dd") },
                            new RtsFhirSubExtension { Url = "status", ValueString = "Active" },
                            new RtsFhirSubExtension { Url = "identifier", ValueString = "ROLE-001" },
                            new RtsFhirSubExtension { Url = "name", ValueString = "ROLE-001_NAME" },
                            new RtsFhirSubExtension
                            {
                                Url = "scoper",
                                ValueReference = new RtsFhirValueReference { Reference = "Organisation/123" }
                            }
                        ]
                    }
                ]
            }
        };

        var response = new RtsOrganisationsAndRolesResponse
        {
            Entry = new List<RtsFhirEntry> { fhirEntry }
        };

        var apiResponse = ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(response);

        var mockRtsClient = Mocker.GetMock<IRtsServiceClient>();

        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 1))
            .ReturnsAsync(ApiResponseHelpers.CreateRtsOrganisationsAndRolesResponse(
                new RtsOrganisationsAndRolesResponse { Total = 1 }
            ));

        mockRtsClient
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>(), 0, 400))
            .ReturnsAsync(apiResponse);

        // Act
        var result = (await _service.GetOrganisationsAndRoles("2024-01-01")).ToList();

        // Assert
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(1);

        result[0].RtsOrganisation.Name.ShouldBe("Test Organisation");
        result[0].RtsOrganisation.Type.ShouldBe("Gov");
        result[0].RtsOrganisation.OId.ShouldBe("ORG-ID-001");

        var role = result[0].RtsRole.First();
        role.Id.ShouldBe("ROLE-001");
        role.Status.ShouldBe("Active");
        role.Scoper.ShouldBe(123);
        role.RoleName.ShouldBe("ROLE-001_NAME");
        role.OrganisationId.ShouldBe("ORG-001");
    }

    [Fact]
    public void TransformOrganisationAndRoles_ReturnsEmpty()
    {
        // Arrange
        var fhirEntry = new RtsFhirEntry();

        // Act
        var result = _service.TransformOrganisationAndRoles(fhirEntry);

        // Assert
        result.RtsOrganisation.ShouldBeNull();
    }

    // Clean up the connection after tests
    ~OrganisationsServiceTests()
    {
        _connection?.Close();
        _connection?.Dispose();
    }
}