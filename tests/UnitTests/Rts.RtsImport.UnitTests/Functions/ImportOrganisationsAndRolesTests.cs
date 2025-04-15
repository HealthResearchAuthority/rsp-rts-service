using Microsoft.AspNetCore.Mvc;
using Moq;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Functions;
using Rts.RtsImport.UnitTests.Helpers;
using Shouldly;

namespace Rts.RtsImport.UnitTests.Functions;

public class ImportOrganisationsAndRolesTests : TestServiceBase
{
    [Fact]
    public async Task Run_ReturnsOk_WithDefaultParameters()
    {
        // Arrange
        var mockImportService = Mocker.GetMock<IOrganisationImportService>();
        mockImportService
            .Setup(x => x.ImportOrganisationsAndRoles(It.IsAny<string>(), false))
            .ReturnsAsync(3);

        var function = Mocker.CreateInstance<ImportOrganisationsAndRoles>();
        var request = HttpHelper.CreateHttpRequest();

        // Act
        var result = await function.Run(request) as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);
        result.Value.ShouldBe("Import complete Updated: 3");
    }

    [Fact]
    public async Task Run_UsesCustomLastUpdatedDate_WhenProvided()
    {
        // Arrange
        var lastUpdated = DateTime.Today.AddDays(-5).ToString("yyyy-MM-dd");

        var mockImportService = Mocker.GetMock<IOrganisationImportService>();
        mockImportService
            .Setup(x => x.ImportOrganisationsAndRoles(lastUpdated, false))
            .ReturnsAsync(2);

        var function = Mocker.CreateInstance<ImportOrganisationsAndRoles>();
        var request = HttpHelper.CreateHttpRequest(new Dictionary<string, string>
        {
            { "_lastUpdated", lastUpdated }
        });

        // Act
        var result = await function.Run(request) as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);
        result.Value.ShouldBe("Import complete Updated: 2");
    }

    [Fact]
    public async Task Run_ReturnsBadRequest_WhenLastUpdatedDateIsInvalid()
    {
        // Arrange
        var function = Mocker.CreateInstance<ImportOrganisationsAndRoles>();
        var request = HttpHelper.CreateHttpRequest(new Dictionary<string, string>
        {
            { "_lastUpdated", "invalid-date" }
        });

        // Act
        var result = await function.Run(request) as BadRequestObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(400);
        result.Value.ShouldBe("_lastUpdated is not provided in the correct format of: 'yyyy-MM-dd'");
    }

    [Fact]
    public async Task Run_OverridesLastUpdated_WhenImportAllRecordsIsTrue()
    {
        // Arrange
        var mockImportService = Mocker.GetMock<IOrganisationImportService>();
        mockImportService
            .Setup(x => x.ImportOrganisationsAndRoles(null, false)) // lastUpdated is null
            .ReturnsAsync(5);

        var function = Mocker.CreateInstance<ImportOrganisationsAndRoles>();
        var request = HttpHelper.CreateHttpRequest(new Dictionary<string, string>
        {
            { "importAllRecords", "true" }
        });

        // Act
        var result = await function.Run(request) as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);
        result.Value.ShouldBe("Import complete Updated: 5");
    }

    [Fact]
    public async Task Run_ReturnsServerError_WhenExceptionThrown()
    {
        // Arrange
        var mockImportService = Mocker.GetMock<IOrganisationImportService>();
        mockImportService
            .Setup(x => x.ImportOrganisationsAndRoles(It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new Exception("Something went wrong"));

        var function = Mocker.CreateInstance<ImportOrganisationsAndRoles>();
        var request = HttpHelper.CreateHttpRequest();

        // Act
        var result = await function.Run(request) as ObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(500);
        result.Value.ShouldBeAssignableTo<object>();
        (result.Value?.ToString() ?? string.Empty).ShouldContain("Something went wrong");
    }

    [Fact]
    public async Task Run_PassesOnlyActiveTrue_WhenQueryParameterIsSet()
    {
        // Arrange
        var expectedDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");

        var mockImportService = Mocker.GetMock<IOrganisationImportService>();
        mockImportService
            .Setup(x => x.ImportOrganisationsAndRoles(expectedDate, true))
            .ReturnsAsync(4);

        var function = Mocker.CreateInstance<ImportOrganisationsAndRoles>();
        var request = HttpHelper.CreateHttpRequest(new Dictionary<string, string>
        {
            { "onlyActive", "true" }
        });

        // Act
        var result = await function.Run(request) as OkObjectResult;

        // Assert
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);
        result.Value.ShouldBe("Import complete Updated: 4");
    }

}