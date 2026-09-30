# 16. Background Services

```csharp
// Service chạy nền — ví dụ: gửi email hàng đợi
public class EmailBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailBackgroundService> _logger;

    public EmailBackgroundService(IServiceProvider serviceProvider,
        ILogger<EmailBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Background Service đang chạy");

        while (!stoppingToken.IsCancellationRequested)
        {
            // Tạo scope mới để lấy Scoped services (DbContext)
            using var scope = _serviceProvider.CreateScope();
            var emailQueue = scope.ServiceProvider.GetRequiredService<IEmailQueue>();

            var pendingEmails = await emailQueue.GetPendingAsync();
            foreach (var email in pendingEmails)
            {
                await emailQueue.SendAsync(email);
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Chờ 30s
        }
    }
}

// Đăng ký
builder.Services.AddHostedService<EmailBackgroundService>();
```

---

[← 15. File Upload](15-file-upload.md) · [Mục lục](README.md) · [17. Clean Architecture →](17-clean-architecture.md)
