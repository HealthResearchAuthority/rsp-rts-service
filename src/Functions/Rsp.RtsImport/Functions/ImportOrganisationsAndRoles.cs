using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.Contracts;

namespace Rsp.RtsImport.Functions;

public class ImportOrganisationsAndRoles
(
    ILogger<ImportOrganisationsAndRoles> logger,
    IOrganisationImportService importService
)
{
    [Function("ImportOrganisationAndRoles")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var onlyActive =
                !string.IsNullOrEmpty(req.Query["onlyActive"]) &&
                bool.Parse(req.Query["onlyActive"]); // if true then only active records will be imported
            var importAllRecords = !string.IsNullOrEmpty(req.Query["importAllRecords"]) &&
                                   bool.Parse(req.Query["importAllRecords"]);

            // last modified date should be yesterday's date. This is to ensure that only data changes since yesterday's pull are retrieved
            var _lastUpdated = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");

            // check if there is a the optional _lastUpdated parameter is sent in the request. This overrides yesterday's date.
            var _lastUpdatedParameter = req.Query["_lastUpdated"];
            if (!string.IsNullOrEmpty(_lastUpdatedParameter))
            {
                // check the parameter lastModified date is in the expected format
                var dateInCorrectFormat = DateTime.TryParseExact(req.Query["_lastUpdated"], "yyyy-MM-dd", null,
                    DateTimeStyles.None, out var date);
                if (dateInCorrectFormat)
                {
                    // use the parameter date instead of the default
                    _lastUpdated = _lastUpdatedParameter;
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
                _lastUpdated = null;
            }

            logger.LogAsInformation("Organisations import started");

            var importOrganisations = await importService.ImportOrganisationsAndRoles(_lastUpdated!, onlyActive);

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
            stopwatch.Stop();
            Console.WriteLine(
                $"Data imported completed in: {stopwatch.Elapsed.Hours:D2}h:{stopwatch.Elapsed.Minutes:D2}m:{stopwatch.Elapsed.Seconds:D2}s");
        }
    }
}