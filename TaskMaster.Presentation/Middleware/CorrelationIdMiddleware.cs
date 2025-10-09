using Serilog.Context;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Usamos el TraceIdentifier de la petición como ID de correlación
        var correlationId = context.TraceIdentifier;

        // Usamos LogContext para "empujar" esta propiedad al contexto del log.
        // Todo lo que se loguee a partir de este punto en la petición, tendrá esta propiedad.
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}