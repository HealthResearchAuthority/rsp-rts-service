using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;

namespace Rsp.RtsService.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProbesController : ControllerBase
{
    private ILogger<ProbesController> _logger;

    public ProbesController(ILogger<ProbesController> logger)
    {
        _logger = logger;
    }
    [HttpGet("liveness")]
    public IActionResult Liveness()
    {
        try
        {
            _logger.LogAsInformation("Sucesfully called the LIVENESS probe.");
            return Ok();
;
        }
        catch (Exception ex)
        {
            _logger.LogAsError("SERVER_ERROROR", ex.Message, ex);
            throw;
        }
    }

    [HttpGet("readiness")]
    public IActionResult Readiness()
    {
        try
        {
            _logger.LogAsInformation("Sucesfully called the READINESS probe.");
            return Ok();
            ;
        }
        catch (Exception ex)
        {
            _logger.LogAsError("SERVER_ERROROR", ex.Message, ex);
            throw;
        }
    }

    [HttpGet("startup")]
    public IActionResult Startup()
    {
        try
        {
            _logger.LogAsInformation("Sucesfully called the STARTUP probe.");
            return Ok();
            ;
        }
        catch (Exception ex)
        {
            _logger.LogAsError("SERVER_ERROROR", ex.Message, ex);
            throw;
        }
    }
}