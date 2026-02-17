using System.Net;
using System.Text.Json;

namespace HelpTrackAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    await WriteErrorResponse(context, HttpStatusCode.NotFound, "Resource not found");
                }
            }
            catch (KeyNotFoundException ex)
            {
                await WriteErrorResponse(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteErrorResponse(context, HttpStatusCode.Forbidden, "Access denied");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "An unexpected server error occurred");
            }
        }

        private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = (int)statusCode,
                message = message,
                timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}