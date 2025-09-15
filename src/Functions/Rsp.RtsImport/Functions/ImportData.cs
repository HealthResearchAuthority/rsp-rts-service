using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.Contracts;

namespace Rsp.RtsImport.Functions;

public class ImportAllData(
    ILogger<ImportAllData> logger,
    IOrganisationImportService importService,
    IMetadataService metadataService,
    IAuditService auditService
)
{
    // function that runs daily at 7AM and checks for updated RTS data.
    [Function("ImportAllData")]
    public async Task<IActionResult> Run(
        [TimerTrigger("%RtsTimerSchedule%", RunOnStartup = true, UseMonitor = true)] TimerInfo myTimer)

    {
        await auditService.FunctionStarted();

        try
        {
            var metadata = await metadataService.GetMetaData();

            var today = DateTime.UtcNow.Date;
            var totalUpdatedRecords = 0;

            if (!string.IsNullOrEmpty(metadata?.LastUpdated))
            {
                var lastUpdatedDate = DateTime.Parse(metadata.LastUpdated, CultureInfo.InvariantCulture).Date;

                if (lastUpdatedDate < today)
                {
                    // Run for all missing days since last update
                    for (var date = lastUpdatedDate.AddDays(1); date <= today; date = date.AddDays(1))
                    {
                        var dateParam = date.ToString("s");
                        var dailyUpdate = await importService.ImportOrganisationsAndRoles(dateParam);
                        totalUpdatedRecords += dailyUpdate;
                    }
                }
            }
            else
            {
                // No LastUpdated -> run with null param
                var dailyUpdate = await importService.ImportOrganisationsAndRoles(null);
                totalUpdatedRecords += dailyUpdate;
            }

            await metadataService.UpdateLastUpdated();

            return new OkObjectResult(
                $"Successfully ran the update process. Total records updated: {totalUpdatedRecords}.");
        }
        catch (Exception ex)
        {
            logger.LogAsError(ErrorStatus.ServerError, ex.Message, ex);

            return new ObjectResult(new
            {
                error = ErrorStatus.ServerError,
                details = ex.Message
            })
            {
                StatusCode = 500
            };
        }
        finally
        {
            await auditService.FunctionEnded();
        }
    }
}