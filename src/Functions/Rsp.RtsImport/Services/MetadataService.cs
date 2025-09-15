using Microsoft.EntityFrameworkCore;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;

namespace Rsp.RtsImport.Services;

public class MetadataService(
    RtsDbContext db) : IMetadataService
{
    public async Task<Metadata?> GetMetaData()
    {
        var record = await db.Metadata.FirstOrDefaultAsync();
        return record;
    }

    public async Task<Metadata> UpdateLastUpdated()
    {
        var record = await db.Metadata.FirstOrDefaultAsync();

        if (record == null)
        {
            record = new Metadata
            {
                LastUpdated = DateTime.UtcNow.ToString("s")
            };

            db.Metadata.Add(record);
        }
        else
        {
            record.LastUpdated = DateTime.UtcNow.ToString("s");
            db.Metadata.Update(record);
        }

        await db.SaveChangesAsync();
        return record;
    }

}