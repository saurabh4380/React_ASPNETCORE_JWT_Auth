using React_ASPNETCORE_JWT_Auth.Models;
using System.Text.Json;

namespace React_ASPNETCORE_JWT_Auth.Middlewares
{
    /// <summary>
    /// Middleware used to handle and log the Global level Errors.
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext httpContext, ILogger<ErrorHandlerMiddleware> logger)
        {
            _logger = logger;

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex, "Global Level Error");

                var response = httpContext.Response;

                response.ContentType = "application/json";

                response.StatusCode = StatusCodes.Status500InternalServerError;

                var result = JsonSerializer.Serialize(new BaseResponseModel() { IsSuccess = false, Errors = new List<string>() { "Contact Dev Team" } });

                await response.WriteAsync(result);

            }
        }

    }

    public static class ErrorHandlerMiddlewareExtensions
    {
        public static void ConfigureGlobalErrorHandlerMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}
