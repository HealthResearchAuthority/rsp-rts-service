using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.Contracts;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure;

namespace Rsp.RtsImport.Services;

public class AuditService(
    RtsDbContext db,
    ILogger<AuditService> logger
    ) : IAuditService
{
    public async Task FunctionStarted()
    {
        await ImportAudit(AuditConstants.FunctionTimerStarted);
    }

    public async Task FunctionEnded()
    {
        await ImportAudit(AuditConstants.FunctionTimerEnded);
    }

    public async Task ApiDownloadStarted()
    {
        await ImportAudit(AuditConstants.ApiDownloadStarted);
    }

    public async Task ApiDownloadCompleted(int recordCount)
    {
        var message = string.Format(AuditConstants.ApiDownloadCompleted, recordCount);
        await ImportAudit(message);
    }

    public async Task DatabaseOrganisationInsertStarted()
    {
        await ImportAudit(AuditConstants.DatabaseOrganisationInsertStarted);
    }

    public async Task DatabaseOrganisationInsertCompleted(int count)
    {
        var message = string.Format(AuditConstants.DatabaseOrganisationInsertCompleted, count);
        await ImportAudit(message);
    }

    public async Task DatabaseOrganisationRolesInsertStarted()
    {
        await ImportAudit(AuditConstants.DatabaseOrganisationRolesInsertStarted);
    }

    public async Task DatabaseOrganisationRolesInsertCompleted(int count)
    {
        var message = string.Format(AuditConstants.DatabaseOrganisationRolesInsertCompleted, count);
        await ImportAudit(message);
    }

    public async Task DatabaseSponsorOrganisationUpdateStarted()
    {
        var message = string.Format(AuditConstants.DatabaseSponsorOrganisationUpdateStarted);
        await ImportAudit(message);
    }

    public async Task DatabaseSponsorOrganisationUpdateCompleted()
    {
        var message = string.Format(AuditConstants.DatabaseSponsorOrganisationUpdateCompleted);
        await ImportAudit(message);
    }

    private async Task ImportAudit(string description)
    {
        await db.Audit.AddAsync(new Audit
        {
            Timestamp = DateTime.UtcNow,
            Description = description
        });

        await db.SaveChangesAsync();

        logger.LogAsInformation(description);
    }
}