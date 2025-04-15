using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Moq;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsImport.Functions;
using Rsp.RtsService.Domain.Entities;
using Shouldly;

namespace Rts.RtsImport.UnitTests.Functions;

public class ImportAllDataTests : TestServiceBase
{
    [Fact]
    public async Task Run_UsesYesterdayAsDefault_WhenMetadataIsEmpty()
    {
        // Arrange
        var yesterday = DateTime.UtcNow.Date.AddDays(-1).ToString("s");

        var mockMeta = Mocker.GetMock<IMetadataService>();
        mockMeta.Setup(x => x.GetMetaData())
            .ReturnsAsync(new Metadata { LastUpdated = null });

        var mockImport = Mocker.GetMock<IOrganisationImportService>();
        mockImport.Setup(x => x.ImportOrganisationsAndRoles(yesterday, true))
            .ReturnsAsync(3);

        var function = Mocker.CreateInstance<ImportAllData>();

        // Act
        var result = await function.Run(new TimerInfo());

        // Assert
        var ok = result as OkObjectResult;
        ok.ShouldNotBeNull();
        ok.StatusCode.ShouldBe(200);
    }

    [Fact]
    public async Task Run_ImportsDataForEachDaySinceLastUpdated()
    {
        // Arrange
        var twoDaysAgo = DateTime.UtcNow.Date.AddDays(-2);
        var yesterday = twoDaysAgo.AddDays(1);
        var today = DateTime.UtcNow.Date;

        var mockMeta = Mocker.GetMock<IMetadataService>();
        mockMeta.Setup(x => x.GetMetaData())
            .ReturnsAsync(new Metadata { LastUpdated = twoDaysAgo.ToString("yyyy-MM-dd") });

        var mockImport = Mocker.GetMock<IOrganisationImportService>();
        mockImport.Setup(x => x.ImportOrganisationsAndRoles(It.IsAny<string>(), true))
            .ReturnsAsync(5); // Pretend each import returns 5 updated records

        var function = Mocker.CreateInstance<ImportAllData>();

        // Act
        var result = await function.Run(new TimerInfo());

        // Assert
        var ok = result as OkObjectResult;
        ok.ShouldNotBeNull();
        ok.StatusCode.ShouldBe(200);
        ok.Value.ShouldBe("Successfully ran the update process. Total records updated: 0.");
    }

    [Fact]
    public async Task Run_SkipsImport_WhenLastUpdatedIsToday()
    {
        // Arrange
        var today = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");

        var mockMeta = Mocker.GetMock<IMetadataService>();
        mockMeta.Setup(x => x.GetMetaData())
            .ReturnsAsync(new Metadata { LastUpdated = today });

        var function = Mocker.CreateInstance<ImportAllData>();

        // Act
        var result = await function.Run(new TimerInfo());

        // Assert
        var ok = result as OkObjectResult;
        ok.ShouldNotBeNull();
        ok.StatusCode.ShouldBe(200);
        ok.Value.ShouldBe("Successfully ran the update process. Total records updated: 0.");
    }

    [Fact]
    public async Task Run_ReturnsServerError_WhenExceptionThrown()
    {
        // Arrange
        var mockMeta = Mocker.GetMock<IMetadataService>();
        mockMeta.Setup(x => x.GetMetaData())
            .ThrowsAsync(new Exception("test-failure"));

        var function = Mocker.CreateInstance<ImportAllData>();

        // Act
        var result = await function.Run(new TimerInfo());

        // Assert
        var err = result as ObjectResult;
        err.ShouldNotBeNull();
        err.StatusCode.ShouldBe(500);
        (err.Value?.ToString() ?? string.Empty).ShouldContain("test-failure");
    }
}