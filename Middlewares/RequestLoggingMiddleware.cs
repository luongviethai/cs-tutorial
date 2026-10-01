using System.Diagnostics;

namespace CS_Tutorial.Middlewares;

public class RequestLoggingMiddleware
{

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = new Stopwatch();

        sw.Start();

        await _next(context);

        sw.Stop();

        _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path} - {sw.ElapsedMilliseconds} ms");
    }
}
