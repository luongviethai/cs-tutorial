# 9. Middleware — Viết Middleware Tùy Chỉnh

## 9.1 Exception Handling Middleware (Bắt buộc có)

```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Chuyển tiếp cho middleware tiếp theo
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new ApiError(ex.Message));
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new ApiError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(
                new ApiError("Đã xảy ra lỗi. Vui lòng thử lại sau."));
        }
    }
}

// Custom Exceptions
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

// Đăng ký middleware (đặt ĐẦU TIÊN trong pipeline)
app.UseMiddleware<GlobalExceptionMiddleware>();
```

## 9.2 Logging Middleware

```csharp
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
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path;

        _logger.LogInformation("→ {Method} {Path}", method, path);

        await _next(context);

        stopwatch.Stop();
        var statusCode = context.Response.StatusCode;

        _logger.LogInformation("← {Method} {Path} → {StatusCode} ({ElapsedMs}ms)",
            method, path, statusCode, stopwatch.ElapsedMilliseconds);
    }
}
```

---

[← 8. DTOs](08-dtos.md) · [Mục lục](README.md) · [10. Authentication & Authorization →](10-authentication-authorization.md)
