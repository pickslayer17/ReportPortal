using System.Text.Json;
using ReportPortal.DAL.Exceptions;

namespace ReportPortal.MiddleWare
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                var (status, message) = Map(ex);

                context.Response.StatusCode = status;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new { message });
                await context.Response.WriteAsync(result);
            }
        }

        // Maps domain exceptions to HTTP status codes. Never echoes stack traces or raw
        // repository messages (which can contain LINQ predicates) to the client.
        private static (int status, string message) Map(Exception ex) => ex switch
        {
            UserNotFoundException => (StatusCodes.Status404NotFound, "User not found."),
            ProjectNotFoundException => (StatusCodes.Status404NotFound, "Project not found."),
            TestNotFoundException => (StatusCodes.Status404NotFound, "Test not found."),
            FolderNotFoundException => (StatusCodes.Status404NotFound, "Folder not found."),
            TestResultNotFoundException => (StatusCodes.Status404NotFound, "Test result not found."),
            EmailAlreadyExistsException => (StatusCodes.Status409Conflict, ex.Message),
            TestWithSuchNameAlreadyExists => (StatusCodes.Status409Conflict, ex.Message),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, "You do not have access to this resource."),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };
    }
}
