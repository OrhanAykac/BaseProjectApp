using Microsoft.AspNetCore.Diagnostics;

namespace BaseProject.WebAPI.Middlewares;

internal sealed class GlobalExceptionHandlerMiddleware(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandlerMiddleware> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception ex, CancellationToken c)
    {
        logger.LogError(ex, "Unhandled exception occurred");

        // Make sure to set the status code before writing to the response body
        httpContext.Response.StatusCode = ex switch
        {
            ApplicationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = ex,
            ProblemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Type = ex.GetType().Name,
                Title = "An error occurred",
                Detail = ex.Message,
            }
        })
            .ConfigureAwait(false);

        return true;
    }
}
