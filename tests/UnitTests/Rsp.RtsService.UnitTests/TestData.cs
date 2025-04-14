using AutoFixture;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;

namespace Rsp.RtsService.UnitTests;

/// <summary>
/// Data seeding class
/// </summary>
public static class TestData
{
    /// <summary>
    /// Seeds the data with specified number of records
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="generator">Test data generator</param>
    /// <param name="records">Number of records to seed</param>
    public static async Task<IList<Organisation>> SeedData(RtsDbContext context, Generator<Organisation> generator, int records)
    {
        // seed data using bogus
        var entities = generator
            .Take(records)
            .ToList();

        // setup 2 records with the word life for testing name search
        entities[1].Name = "life sciences";
        entities[2].Name = "simple life";

        entities.ForEach(x=>x.Status = true);

        await context.Organisation.AddRangeAsync(entities);

        await context.SaveChangesAsync();

        return entities;
    }
}