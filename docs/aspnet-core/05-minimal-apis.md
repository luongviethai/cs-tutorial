# 5. Minimal APIs — Cách Viết Gọn

Từ .NET 6+, bạn có thể viết API mà không cần Controller. Phù hợp cho API nhỏ, microservice.

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Định nghĩa endpoint trực tiếp trong Program.cs
var products = app.MapGroup("/api/products");  // Nhóm route

products.MapGet("/", async (IProductService service, int page = 1, int pageSize = 10) =>
{
    var result = await service.GetAllAsync(page, pageSize);
    return Results.Ok(result);
});

products.MapGet("/{id:int}", async (int id, IProductService service) =>
{
    var product = await service.GetByIdAsync(id);
    return product is not null
        ? Results.Ok(product)
        : Results.NotFound();
});

products.MapPost("/", async (CreateProductRequest request, IProductService service) =>
{
    var product = await service.CreateAsync(request);
    return Results.Created($"/api/products/{product.Id}", product);
});

products.MapPut("/{id:int}", async (int id, UpdateProductRequest request, IProductService service) =>
{
    await service.UpdateAsync(id, request);
    return Results.NoContent();
});

products.MapDelete("/{id:int}", async (int id, IProductService service) =>
{
    await service.DeleteAsync(id);
    return Results.NoContent();
});

app.Run();
```

> **Controller vs Minimal API:** Controller phù hợp cho dự án lớn, nhiều endpoint, cần tổ chức rõ ràng. Minimal API phù hợp cho microservice nhỏ, prototype nhanh. Cả hai đều chạy trên cùng một nền tảng ASP.NET Core.

---

[← 4. Controller](04-controller.md) · [Mục lục](README.md) · [6. Dependency Injection →](06-dependency-injection.md)
