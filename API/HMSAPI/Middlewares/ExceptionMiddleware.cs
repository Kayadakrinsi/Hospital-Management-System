using NLog;
using System.Net;
using System.Text.Json;

namespace HMSAPI.Middlewares
{
    /// <summary>
    /// Exception handling middleware for global error catching and logging
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Processes an HTTP request and handles any unhandled exceptions that occur during the request pipeline
        /// </summary>
        /// <remarks>If an exception is thrown during request processing, the exception is logged and an
        /// appropriate response is generated. This method should be used as part of the ASP.NET Core middleware
        /// pipeline</remarks>
        /// <param name="context">The HTTP context for the current request</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await LogExceptionDetailsAsync(context, ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Asynchronously logs detailed information about an unhandled exception that occurred during the processing of
        /// an HTTP request
        /// </summary>
        /// <remarks>The log entry includes structured details such as the HTTP method, request path, user
        /// identity, query string, response status code, exception type, message, and stack trace. Logging is performed
        /// in a structured JSON format to facilitate searching and analysis</remarks>
        /// <param name="context">The current HTTP context containing request and response information associated with the exception</param>
        /// <param name="ex">The exception to log, including its message and stack trace</param>
        /// <returns></returns>
        private static async Task LogExceptionDetailsAsync(HttpContext context, Exception ex)
        {
            var request = context.Request;
            var user = context.User?.Identity?.IsAuthenticated == true
                ? context.User.Identity?.Name
                : "Anonymous";

            var logData = new
            {
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                Method = request.Method,
                Path = request.Path,
                User = user,
                QueryString = request.QueryString.HasValue ? request.QueryString.Value : null,
                StatusCode = context.Response?.StatusCode,
                Exception = ex.GetType().Name,
                Message = ex.Message,
                StackTrace = ex.StackTrace
            };

            // Structured JSON-style log for easier searching
            var logJson = JsonSerializer.Serialize(logData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            Logger _logger = LogManager.GetLogger("errorLog");
            _logger.Error(ex, $"[EXCEPTION LOG] {logJson}");

            // Optional: also store for debugging
            await Task.CompletedTask;
        }

        /// <summary>
        /// Writes a JSON-formatted error response to the HTTP context for an unhandled exception
        /// </summary>
        /// <remarks>The response includes the exception message and stack trace in JSON format, and sets
        /// the HTTP status code to 500 (Internal Server Error)</remarks>
        /// <param name="context">The HTTP context for the current request. The response will be written to this context</param>
        /// <param name="ex">The exception that occurred and will be reported in the response</param>
        /// <returns></returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var response = new
            {
                IsError = true,
                Message = ex.Message,
                StackTrace = ex.StackTrace
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(result);
        }
    }

    /// <summary>
    /// Provides extension methods for configuring global exception handling middleware in an ASP.NET Core application
    /// </summary>
    /// <remarks>This class contains extension methods for the IApplicationBuilder interface to simplify the
    /// registration of middleware that handles unhandled exceptions globally. Use these methods in the application's
    /// request pipeline configuration to ensure consistent error handling across all requests.</remarks>
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
