# 2. Cài Đặt Và Tạo Project Đầu Tiên

## 2.1 Cài đặt

```bash
# Tải .NET SDK từ https://dotnet.microsoft.com/download

# Kiểm tra đã cài chưa
dotnet --version
```

## 2.2 Tạo project Web API

```bash
# Tạo project Web API dùng Controller
dotnet new webapi -n MyFirstApi --use-controllers
cd MyFirstApi
```

> **Lưu ý (.NET 8 trở lên):** Nếu bỏ `--use-controllers`, template sẽ tạo project kiểu Minimal API (xem [bài 5](05-minimal-apis.md)) và không có thư mục `Controllers/`.

```text
# Cấu trúc thư mục
MyFirstApi/
├── Controllers/
│   └── WeatherForecastController.cs   # Controller mẫu
├── Properties/
│   └── launchSettings.json            # Cấu hình chạy local (port, môi trường)
├── appsettings.json                   # Cấu hình ứng dụng
├── appsettings.Development.json       # Cấu hình cho môi trường dev
├── MyFirstApi.http                    # File gửi request thử API (mở bằng Visual Studio / VS Code)
├── Program.cs                         # Entry point — CỰC KỲ QUAN TRỌNG
├── WeatherForecast.cs                 # Model mẫu
└── MyFirstApi.csproj                  # File project (dependencies, target framework)
```

## 2.3 Bật giao diện Swagger UI

Từ .NET 9, template không còn kèm Swagger. Project chỉ sinh file mô tả API dạng JSON tại `/openapi/v1.json` (qua `AddOpenApi()` / `MapOpenApi()`). Muốn có giao diện để bấm thử API, cài thêm Swagger UI:

```bash
dotnet add package Swashbuckle.AspNetCore.SwaggerUI
```

Mở `Program.cs`, thêm `UseSwaggerUI` vào khối `if (app.Environment.IsDevelopment())`:

```csharp
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}
```

```bash
# Chạy project
dotnet run

# Terminal in ra địa chỉ, ví dụ: Now listening on: http://localhost:5194
# Mở trình duyệt: http://localhost:5194/swagger
```

> Port được sinh ngẫu nhiên khi tạo project, xem trong `Properties/launchSettings.json`. `dotnet run` dùng profile đầu tiên (`http`); muốn chạy HTTPS thì dùng `dotnet run --launch-profile https`.

## 2.4 Các lệnh dotnet CLI thường dùng

```bash
dotnet new webapi -n TenProject --use-controllers  # Tạo project Web API (dùng Controller)
dotnet new sln -n TenSolution          # Tạo solution
dotnet sln add ./TenProject            # Thêm project vào solution
dotnet run                             # Build và chạy
dotnet watch run                       # Chạy + tự reload khi sửa code (hot reload)
dotnet build                           # Chỉ build, không chạy
dotnet publish -c Release              # Publish bản production
dotnet add package TenPackage          # Cài NuGet package
dotnet restore                         # Restore tất cả packages
dotnet ef migrations add TenMigration  # Tạo EF migration
dotnet ef database update              # Áp dụng migration vào database
```

---

[← 1. ASP.NET Core Là Gì?](01-aspnet-core-la-gi.md) · [Mục lục](README.md) · [3. Program.cs →](03-program-cs.md)
