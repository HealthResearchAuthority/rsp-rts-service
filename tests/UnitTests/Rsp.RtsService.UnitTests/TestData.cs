using AutoFixture;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;

namespace Rsp.RtsService.UnitTests;

/// <summary>
/// Data seeding class
/// </summary>
internal static class TestData
{
    /// <summary>
    /// Seeds the data with specified number of records
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="generator">Test data generator</param>
    /// <param name="records">Number of records to seed</param>
    /// <param name="updateStatus">
    /// Indicates whether to update the pending status. If true, index 2 and 4 will be updated
    /// </param>
    public static async Task<IList<Entity>> SeedData(DbContext context, Generator<Entity> generator, int records, bool updateStatus = false)
    {
        // seed data using bogus
        var entities = generator
            .Take(records)
            .ToList();

        if (updateStatus)
        {
            // set the to something for testing for a few records
            // entities[2].property = "somevalue"
            // entities[4].property = "somevalue"
        }

        await context.Entities.AddRangeAsync(entities);

        await context.SaveChangesAsync();

        return entities;
    }
}