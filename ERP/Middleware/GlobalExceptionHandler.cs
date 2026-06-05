using Erp.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception , CancellationToken cancellationToken)
        {
            // 1. Log the critical error so you have a record of it
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            // 2. Determine the HTTP Status Code based on the exact exception type
            var statusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError // The ultimate fallback
            };
            // 3. Format the response using the industry-standard ProblemDetails
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = GetTitle(statusCode),
                Detail = statusCode == 500 ? "An unexpected error occurred on our end." : exception.Message,
                Instance = httpContext.Request.Path
            };
            // 4. Send it back to the React frontend
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            // Return true to tell .NET "I handled this, stop processing"
            return true;
        }
        private static string GetTitle(int statusCode) => statusCode switch
        {
            400 => "Bad Request",
            404 => "Not Found",
            _ => "Server Error"
        };
    }
}
