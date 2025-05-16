using Moq;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Application.DTO;
using Rsp.RtsImport.Services;
using Rsp.RtsService.Domain.Entities;
using Shouldly;

namespace Rts.RtsImport.UnitTests.Services;

public class OrganisationImportServiceTests : TestServiceBase
{
    [Fact]
    public async Task ImportOrganisationsAndRoles_UpdatesOrganisationsAndRolesAndReturnsTotalCount()
    {
        // Arrange
        var organisation = new Organisation
        {
            Id = "ORG-001",
            OId = "ORG-ID-001",
            Name = "Test Org",
            Type = "Gov",
            TypeId = "GOV001",
            TypeName = "Government",
            Status = true,
            LastUpdated = DateTime.UtcNow,
            SystemUpdated = DateTime.UtcNow,
            Imported = DateTime.UtcNow
        };

        var role = new OrganisationRole
        {
            Id = "ROLE-001",
            OrganisationId = "ORG-001",
            Status = "Active",
            Scoper = 1,
            StartDate = DateTime.UtcNow,
            Imported = DateTime.UtcNow,
            SystemUpdated = DateTime.UtcNow,
        };

        var orgAndRoles = new List<RtsOrganisationAndRole>
        {
            new()
            {
                RtsOrganisation = organisation,
                RtsRole = [role]
            }
        };

        var mockOrgService = Mocker.GetMock<IOrganisationService>();
        mockOrgService
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>()))
            .ReturnsAsync(orgAndRoles);

        mockOrgService
            .Setup(x => x.UpdateOrganisations(It.IsAny<IEnumerable<Organisation>>(), false))
            .ReturnsAsync(new DbOperationResult { RecordsUpdated = 1 });

        mockOrgService
            .Setup(x => x.UpdateRoles(It.IsAny<IEnumerable<OrganisationRole>>(), false))
            .ReturnsAsync(new DbOperationResult { RecordsUpdated = 2 });

        var service = Mocker.CreateInstance<OrganisationImportService>();

        // Act
        var result = await service.ImportOrganisationsAndRoles("2024-01-01");

        // Assert
        result.ShouldBe(3); // 1 org + 2 roles
    }

    [Fact]
    public async Task ImportOrganisationsAndRoles_ReturnsZero_WhenNoOrganisationsOrRolesReturned()
    {
        // Arrange
        var mockOrgService = Mocker.GetMock<IOrganisationService>();
        mockOrgService
            .Setup(x => x.GetOrganisationsAndRoles(It.IsAny<string>()))
            .ReturnsAsync([]); // Empty response

        var service = Mocker.CreateInstance<OrganisationImportService>();

        // Act
        var result = await service.ImportOrganisationsAndRoles("2024-01-01");

        // Assert
        result.ShouldBe(0);
    }
}