using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;

namespace Rts.RtsImport.UnitTests;

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
    public static async Task<IList<Organisation>> SeedOrganisations(RtsDbContext context, Generator<Organisation> generator, int records)
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

    /// <summary>
    /// Seeds the data with specified number of records
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="generator">Test data generator</param>
    /// <param name="records">Number of records to seed</param>
    public static async Task<IList<OrganisationRole>> SeedOrganisationRoles(
        RtsDbContext context,
        Generator<OrganisationRole> generator,
        int records, bool active = false)
    {
        // Ensure we have organisations in the DB
        var existingOrgIds = await context.Organisation.Select(o => o.Id).ToListAsync();

        // Generate roles and assign valid OrganisationIds
        var entities = generator
            .Take(records)
            .ToList();

        // Assign valid OrganisationIds to each role
        var random = new Random();
        foreach (var entity in entities)
        {
            entity.OrganisationId = existingOrgIds[random.Next(existingOrgIds.Count)];
            entity.Status = active ? RtsRecordStatusOptions.ActiveRole : "";
        }

        await context.OrganisationRole.AddRangeAsync(entities);
        await context.SaveChangesAsync();

        return entities;
    }

}