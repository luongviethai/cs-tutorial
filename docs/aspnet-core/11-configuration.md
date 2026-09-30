# 11. Configuration (Cấu hình)

## 11.1 appsettings.json

```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=MyApp;Trusted_Connection=true;"
    },
    "JwtSettings": {
        "SecretKey": "change-this-in-production",
        "Issuer": "MyApp",
        "Audience": "MyApp",
        "ExpirationInMinutes": 60
    },
    "EmailSettings": {
        "SmtpHost": "smtp.gmail.com",
        "SmtpPort": 587,
        "SenderEmail": "app@example.com"
    },
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning",
            "Microsoft.EntityFrameworkCore": "Information"
        }
    }
}
```

## 11.2 Đọc configuration

```csharp
// --- Cách 1: Options Pattern (khuyến khích) ---
public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
}

// Đăng ký
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Sử dụng
public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value; // Lấy giá trị từ configuration
    }
}

// --- Cách 2: Đọc trực tiếp ---
var secretKey = builder.Configuration["JwtSettings:SecretKey"];
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// --- Environment-specific config ---
// appsettings.Development.json    → Dùng khi chạy local
// appsettings.Production.json     → Dùng khi deploy
// Environment variables           → Override tất cả (ưu tiên cao nhất)
```

---

[← 10. Authentication & Authorization](10-authentication-authorization.md) · [Mục lục](README.md) · [12. FluentValidation →](12-fluent-validation.md)
