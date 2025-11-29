using NLog;
using System.Net;
using System.Text.Json;

namespace HMSAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

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

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
