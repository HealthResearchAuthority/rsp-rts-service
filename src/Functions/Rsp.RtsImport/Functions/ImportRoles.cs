using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Application.Contracts;

namespace Rsp.RtsImport.Functions;

public class ImportRoles
{
    private readonly ILogger<ImportRoles> _logger;
    private readonly IOrganisationImportService _importService;

    public ImportRoles(ILogger<ImportRoles> logger,
        IOrganisationImportService importService)
    {
        _logger = logger;
        _importService = importService;
    }

    [Function("ImportRoles")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        try
        {
            var onlyActive = !string.IsNullOrEmpty(req.Query["onlyActive"]) && bool.Parse(req.Query["onlyActive"]); // if true then only active records will be imported
            var importAllRecords = string.IsNullOrEmpty(req.Query["importAllRecords"]) ? false : bool.Parse(req.Query["importAllRecords"]);

            // last modified date should be yesterday's date. This is to ensure that only data changes since yesterday's pull are retrieved
            var dateModified = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");

            // check if there is a the optional dateModified parameter is sent in the request. This overrides yesterday's date.
            var dateModifiedParameter = req.Query["dateModified"];
            if (!string.IsNullOrEmpty(dateModifiedParameter))
            {
                // check the parameter lastModified date is in the expected format
                var dateInCorrectFormat = DateTime.TryParseExact(req.Query["dateModified"], "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime date);
                if (dateInCorrectFormat)
                {
                    // use the parameter date instead of the default
                    dateModified = dateModifiedParameter;
                }
                else
                {
                    _logger.LogAsError(ErrorStatus.BadRequest, "dateModified is not provided in the correct format of: 'yyyy-MM-dd'");
                    return new BadRequestObjectResult("dateModified is not provided in the correct format of: 'yyyy-MM-dd'");
                }
            }

            // if importAllRecords parameter is a true then invalidate import date to get all records from API
            if (importAllRecords)
            {
                dateModified = null;
            }

            _logger.LogAsInformation("Roles import started");

            var importRoles = await _importService.ImportRoles(dateModified!, onlyActive);

            return new OkObjectResult($"Import complete Updated: {importRoles} items");
        }
        catch (Exception ex)
        {
            _logger.LogAsError(ErrorStatus.ServerError, ex.Message, ex);
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