namespace TUnitTesting.WebApi.Middleware;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        logger.LogDebug("Incoming Request: {Method} {Path}", context.Request.Method, context.Request.Path);

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation(
                "API Request: {Method} {Path} {StatusCode} in {ElapsedMs}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds
            );
        }
    }
}