# 6. Dependency Injection (DI) — Chi Tiết

DI là pattern cốt lõi nhất trong ASP.NET Core. Hiểu DI = hiểu ASP.NET Core.

## 6.1 Tại sao cần DI?

```csharp
// ❌ KHÔNG có DI — Tightly coupled (phụ thuộc chặt)
public class ProductsController
{
    public ActionResult GetAll()
    {
        // Controller tự tạo mọi thứ → Không thể test, không thể đổi implementation
        var context = new AppDbContext(/* phải tự truyền options */);
        var logger = new ConsoleLogger();
        var products = context.Products.ToList();
        return Ok(products);
    }
}

// ✅ CÓ DI — Loosely coupled (phụ thuộc lỏng)
public class ProductsController
{
    private readonly IProductService _service; // Chỉ biết interface, KHÔNG biết implementation

    public ProductsController(IProductService service) // Framework tự tiêm vào
    {
        _service = service;
    }

    public ActionResult GetAll()
    {
        var products = _service.GetAll(); // Gọi qua interface
        return Ok(products);
    }
}
```

## 6.2 Ba loại Lifetime

```csharp
// --- TRANSIENT: Tạo MỚI mỗi khi được yêu cầu ---
builder.Services.AddTransient<IEmailService, EmailService>();
// Dùng cho: Service nhẹ, không giữ state
// Ví dụ: EmailService, PdfGenerator, Validator

// --- SCOPED: Tạo MỘT lần cho mỗi HTTP request ---
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Dùng cho: Service làm việc với database, business logic
// Phổ biến nhất! DbContext mặc định cũng là Scoped

// --- SINGLETON: Tạo MỘT lần duy nhất cho toàn bộ ứng dụng ---
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
// Dùng cho: Cache, configuration, HttpClient factory
// Cẩn thận: Thread-safe bắt buộc!
```

## 6.3 Đăng ký DI thực tế

> `ProductDto` và `CreateProductRequest` được định nghĩa ở [bài 8](08-dtos.md).

```csharp
// Interface
public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductRequest request);
}

// Implementation
public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext context, ILogger<ProductService> logger)
    {
        _context = context;   // DbContext cũng được DI tiêm vào
        _logger = logger;     // Logger cũng được DI tiêm vào
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        return await _context.Products
            .Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price })
            .ToListAsync();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product is null
            ? null
            : new ProductDto { Id = product.Id, Name = product.Name, Price = product.Price };
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            CategoryId = request.CategoryId   // Bắt buộc: Product luôn thuộc một Category
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Tạo sản phẩm mới: {ProductName}", product.Name);

        return new ProductDto { Id = product.Id, Name = product.Name, Price = product.Price };
    }
}

// Đăng ký trong Program.cs
builder.Services.AddScoped<IProductService, ProductService>();
```

---

[← 5. Minimal APIs](05-minimal-apis.md) · [Mục lục](README.md) · [7. Entity Framework Core →](07-entity-framework-core.md)
