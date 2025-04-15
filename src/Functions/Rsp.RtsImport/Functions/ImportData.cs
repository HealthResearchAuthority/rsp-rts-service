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
    IMetadataService metadataService
)
{
    // function that runs daily at 7AM and checks for updated RTS data.
    [Function("ImportAllData")]
    public async Task<IActionResult> Run(
        [TimerTrigger("0 0 7 * * *")] TimerInfo _)

    {
        try
        {
            var metadata = await metadataService.GetMetaData();

            var lastUpdatedDate = string.IsNullOrEmpty(metadata?.LastUpdated)
                ? DateTime.UtcNow.Date.AddDays(-1) // default to yesterday if missing
                : DateTime.Parse(metadata.LastUpdated, CultureInfo.InvariantCulture).Date;

            var today = DateTime.UtcNow.Date;

            var totalUpdatedRecords = 0;

            if (lastUpdatedDate < today)
            {
                // WILL ONLY RUN FOR DAYS NOT ALREADY RUN
                for (var date = lastUpdatedDate.AddDays(1); date <= today; date = date.AddDays(1))
                {
                    var dateParam = date.ToString("s");
                    var dailyUpdate = await importService.ImportOrganisationsAndRoles(dateParam);
                    totalUpdatedRecords += dailyUpdate;
                }
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
    }
}