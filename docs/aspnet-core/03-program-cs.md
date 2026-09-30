# 3. Program.cs — Trái Tim Của Ứng Dụng

Đây là file quan trọng nhất. Mọi cấu hình đều bắt đầu từ đây.

```csharp
// ==========================================
//  PHẦN 1: BUILDER — Đăng ký Services
// ==========================================

var builder = WebApplication.CreateBuilder(args);

// --- Đăng ký Controllers ---
builder.Services.AddControllers();

// --- Đăng ký OpenAPI (tài liệu API tự động) ---
builder.Services.AddOpenApi();

// --- Đăng ký Database (Entity Framework Core) ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Đăng ký Dependency Injection ---
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// --- Đăng ký Authentication (JWT) ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* cấu hình JWT */ });

// --- Đăng ký CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // URL frontend React/Vue
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ==========================================
//  PHẦN 2: MIDDLEWARE PIPELINE — Xử lý Request
// ==========================================

// THỨ TỰ RẤT QUAN TRỌNG! Request đi qua từng middleware từ trên xuống.

// Chỉ bật tài liệu API trong môi trường Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                   // File JSON mô tả API tại /openapi/v1.json
    app.UseSwaggerUI(options =>         // Giao diện thử API tại /swagger
        options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseHttpsRedirection();      // Chuyển HTTP → HTTPS
app.UseCors("AllowFrontend");   // Xử lý CORS
app.UseAuthentication();        // Xác thực: Ai đang gọi? (phải trước Authorization)
app.UseAuthorization();         // Phân quyền: Có được phép không?
app.MapControllers();           // Map route đến controllers

app.Run();
```

> **Swagger từ .NET 9:** Template dùng `AddOpenApi()` / `MapOpenApi()` (package `Microsoft.AspNetCore.OpenApi`, có sẵn trong project) thay cho `AddSwaggerGen()` / `UseSwagger()` của Swashbuckle. `UseSwaggerUI` cần cài thêm package `Swashbuckle.AspNetCore.SwaggerUI` — xem [bài 2](02-cai-dat-va-tao-project.md), mục 2.3.

## Giải thích luồng hoạt động

```text
Client gửi HTTP Request
        ↓
   HTTPS Redirect         → Chuyển http → https
        ↓
      CORS                → Kiểm tra origin có được phép không
        ↓
   Authentication         → Đọc JWT token, xác minh người dùng
        ↓
   Authorization          → Kiểm tra quyền truy cập
        ↓
     Routing              → Tìm controller/action phù hợp
        ↓
   Controller Action      → Xử lý logic, trả về response
        ↓
Client nhận HTTP Response
```

---

[← 2. Cài Đặt & Tạo Project](02-cai-dat-va-tao-project.md) · [Mục lục](README.md) · [4. Controller →](04-controller.md)
