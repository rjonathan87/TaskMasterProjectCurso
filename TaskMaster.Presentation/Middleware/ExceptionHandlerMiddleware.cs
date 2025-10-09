using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace TaskMaster.Presentation.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió una excepción no manejada: {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Creamos un objeto ProblemDetails estándar
            var problemDetails = new ProblemDetails
            {
                Status = context.Response.StatusCode,
                Title = "Se produjo un error interno en el servidor.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                // Incluimos el detalle del error solo en desarrollo por seguridad
                Detail = _env.IsDevelopment() ? exception.ToString() : "Contacte a soporte."
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }
    }
}