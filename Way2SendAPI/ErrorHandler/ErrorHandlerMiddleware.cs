using System.Net;

namespace Way2SendAPI.ErrorHandler
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

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił nieoczekiwany błąd.");
                await HandleExceptionAsync(httpContext);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Wystąpił błąd serwera. Spróbuj ponownie później lub skontaktuj się z administratorem."
            }.ToString());
        }
    }
}
