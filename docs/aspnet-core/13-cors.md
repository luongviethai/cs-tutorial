# 13. CORS (Cross-Origin Resource Sharing)

CORS cho phép frontend (React, Vue...) ở domain khác gọi API của bạn.

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    // Policy cho Development
    options.AddPolicy("Development", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",      // React dev server
                "http://localhost:5173")      // Vite dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();            // Cho phép gửi cookie/token
    });

    // Policy cho Production
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://myapp.com", "https://www.myapp.com")
              .WithHeaders("Authorization", "Content-Type")
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .AllowCredentials();
    });
});

// Chọn policy theo môi trường
if (app.Environment.IsDevelopment())
    app.UseCors("Development");
else
    app.UseCors("Production");
```

---

[← 12. FluentValidation](12-fluent-validation.md) · [Mục lục](README.md) · [14. Logging →](14-logging.md)
