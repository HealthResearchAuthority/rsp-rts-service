using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.Contracts;
using static System.Boolean;

namespace Rsp.RtsImport.Functions;

public class ImportOrganisationsAndRoles(
    ILogger<ImportOrganisationsAndRoles> logger,
    IOrganisationImportService importService,
    IAuditService auditService
)
{
    [Function("ImportOrganisationAndRoles")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        await auditService.FunctionStarted();

        try
        {
            var onlyActive = false;
            if (req?.Query.TryGetValue("onlyActive", out var onlyActiveValue) == true &&
                TryParse(onlyActiveValue.FirstOrDefault(), out var parsedOnlyActive))
            {
                onlyActive = parsedOnlyActive;
            }

            var importAllRecords = false;
            if (req?.Query.TryGetValue("importAllRecords", out var importAllRecordsValue) == true &&
                TryParse(importAllRecordsValue.FirstOrDefault(), out var parsedImportAll))
            {
                importAllRecords = parsedImportAll;
            }

            // last modified date should be yesterday's date. This is to ensure that only data changes since yesterday's pull are retrieved
            var lastUpdated = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");

            // check if there is a the optional _lastUpdated parameter is sent in the request. This overrides yesterday's date.
            var lastUpdatedParameter = req?.Query["_lastUpdated"];
            if (!string.IsNullOrEmpty(lastUpdatedParameter))
            {
                // check the parameter lastModified date is in the expected format
                var dateInCorrectFormat = DateTime.TryParseExact(
                    req?.Query["_lastUpdated"],
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _);

                if (dateInCorrectFormat)
                {
                    // use the parameter date instead of the default
                    lastUpdated = lastUpdatedParameter;
                }
                else
                {
                    logger.LogAsError(ErrorStatus.BadRequest,
                        "_lastUpdated is not provided in the correct format of: 'yyyy-MM-dd'");
                    return new BadRequestObjectResult(
                        "_lastUpdated is not provided in the correct format of: 'yyyy-MM-dd'");
                }
            }

            // if importAllRecords parameter is a true then invalidate import date to get all records from API
            if (importAllRecords)
            {
                lastUpdated = null;
            }

            logger.LogAsInformation("Organisations import started");

            var importOrganisations = await importService.ImportOrganisationsAndRoles(lastUpdated!, onlyActive);

            return new OkObjectResult($"Import complete Updated: {importOrganisations}");
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