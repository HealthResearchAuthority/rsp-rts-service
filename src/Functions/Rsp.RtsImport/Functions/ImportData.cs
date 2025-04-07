namespace Rsp.RtsImport.Functions;

public class ImportAllData
{
    private readonly ILogger<ImportAllData> _logger;
    private readonly IOrganisationImportService _importService;

    public ImportAllData(ILogger<ImportAllData> logger,
        IOrganisationImportService importService)
    {
        _logger = logger;
        _importService = importService;
    }

    // function that runs daily at 7AM and checks for updated RTS data.
    //[Function("ImportAllData")]
    public async Task<IActionResult> Run(
        [TimerTrigger("0 0 7 * * *")] TimerInfo myTimer)
    {
        try
        {
            // get yesterday's date so we only get the delta between today and yesterday
            var _lastUpdated = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");

            // order of data retrieval is important due to foreign key constrains
            // Step 1: Import organisation termsets
            var updatedOrganisationAndRoles = await _importService.ImportOrganisationsAndRoles(_lastUpdated);

            return new OkObjectResult($"Sucesfully ran the update process. Total records updated: {updatedOrganisationAndRoles}.");
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