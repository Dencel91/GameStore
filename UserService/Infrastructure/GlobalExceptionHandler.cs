using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Infrastructure;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case ArgumentException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad request";
                problemDetails.Detail = exception.Message;
                break;

            case InvalidOperationException:
            default:

                logger.LogError("Unhandled error: {errorMessage}", exception.Message);
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Server error";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status ?? httpContext.Response.StatusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
