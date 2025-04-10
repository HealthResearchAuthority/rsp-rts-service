namespace Rsp.RtsImport.Infrastructure;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogAsError(ErrorStatus.ServerError, "Error processing invocation", ex); // Constant for error here.
        }
    }
}