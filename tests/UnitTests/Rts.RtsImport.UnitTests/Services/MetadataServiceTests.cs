using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Rsp.RtsImport.Services;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;

namespace Rts.RtsImport.UnitTests.Services;

public class MetadataServiceTests : TestServiceBase
{
    private readonly MetadataService _service;

    public MetadataServiceTests()
    {
        var options = new DbContextOptionsBuilder<RtsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        var context = new RtsDbContext(options);

        // Seed a Metadata record
        context.Metadata.Add(new Metadata
        {
            Id = 1,
            LastUpdated = "2023-01-01T00:00:00"
        });
        context.SaveChanges();

        _service = new MetadataService(context);
    }

    [Fact]
    public async Task GetMetaData_Returns_Metadata()
    {
        // Act
        var result = await _service.GetMetaData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("2023-01-01T00:00:00", result.LastUpdated);
    }

    [Fact]
    public async Task UpdateLastUpdated_Updates_LastUpdated_Field()
    {
        // Act
        var result = await _service.UpdateLastUpdated();

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual("2023-01-01T00:00:00", result.LastUpdated);

        // Optionally verify it's in ISO 8601 format (e.g., "2025-04-15T13:00:00")
        Assert.True(DateTime.TryParseExact(
            result.LastUpdated,
            "yyyy-MM-ddTHH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _));
    }

    [Fact]
    public async Task UpdateLastUpdated_Returns_Null_When_No_Metadata_Exists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RtsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        var context = new RtsDbContext(options); // No seeding

        var service = new MetadataService(context);

        // Act
        var result = await service.UpdateLastUpdated();

        // Assert
        Assert.Null(result);
    }
}