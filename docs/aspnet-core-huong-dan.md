# Hướng Dẫn ASP.NET Core — Xây Dựng Web API Từ Zero

---

## 1\. ASP.NET Core Là Gì?

ASP.NET Core là framework web mã nguồn mở, đa nền tảng (Windows, macOS, Linux) của Microsoft. Nó được thiết kế để xây dựng các ứng dụng web hiện đại, API, real-time, và microservices với hiệu năng cực cao (thường xuyên nằm top benchmark các web framework).

### Tại sao chọn ASP.NET Core?

- **Hiệu năng cao:** Nằm trong top nhanh nhất theo benchmark TechEmpower.  
- **Đa nền tảng:** Chạy trên Windows, macOS, Linux.  
- **Hệ sinh thái mạnh:** Entity Framework Core, Identity, SignalR, gRPC...  
- **Dependency Injection tích hợp sẵn:** Không cần thư viện bên ngoài.  
- **Bảo mật tốt:** Tích hợp sẵn Authentication, Authorization, CORS, HTTPS...  
- **Tuyển dụng cao:** Đặc biệt trong enterprise, ngân hàng, fintech.

---

## 2\. Cài Đặt Và Tạo Project Đầu Tiên

### 2.1 Cài đặt

\# Tải .NET SDK từ https://dotnet.microsoft.com/download

\# Kiểm tra đã cài chưa

dotnet \--version

### 2.2 Tạo project Web API

\# Tạo project Web API

dotnet new webapi \-n MyFirstApi

cd MyFirstApi

\# Cấu trúc thư mục

MyFirstApi/

├── Controllers/

│   └── WeatherForecastController.cs   \# Controller mẫu

├── Properties/

│   └── launchSettings.json            \# Cấu hình chạy local

├── appsettings.json                   \# Cấu hình ứng dụng

├── appsettings.Development.json       \# Cấu hình cho môi trường dev

├── Program.cs                         \# Entry point — CỰC KỲ QUAN TRỌNG

└── MyFirstApi.csproj                  \# File project (dependencies, target framework)

\# Chạy project

dotnet run

\# Mở trình duyệt: https://localhost:5001/swagger

### 2.3 Các lệnh dotnet CLI thường dùng

dotnet new webapi \-n TenProject     \# Tạo project Web API

dotnet new sln \-n TenSolution       \# Tạo solution

dotnet sln add ./TenProject         \# Thêm project vào solution

dotnet run                          \# Build và chạy

dotnet watch run                    \# Chạy \+ tự reload khi sửa code (hot reload)

dotnet build                        \# Chỉ build, không chạy

dotnet publish \-c Release           \# Publish bản production

dotnet add package TenPackage       \# Cài NuGet package

dotnet restore                      \# Restore tất cả packages

dotnet ef migrations add TenMigration  \# Tạo EF migration

dotnet ef database update           \# Áp dụng migration vào database

---

## 3\. Program.cs — Trái Tim Của Ứng Dụng

Đây là file quan trọng nhất. Mọi cấu hình đều bắt đầu từ đây.

// \==========================================

//  PHẦN 1: BUILDER — Đăng ký Services

// \==========================================

var builder \= WebApplication.CreateBuilder(args);

// \--- Đăng ký Controllers \---

builder.Services.AddControllers();

// \--- Đăng ký Swagger (tài liệu API tự động) \---

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// \--- Đăng ký Database (Entity Framework Core) \---

builder.Services.AddDbContext\<AppDbContext\>(options \=\>

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// \--- Đăng ký Dependency Injection \---

builder.Services.AddScoped\<IProductService, ProductService\>();

builder.Services.AddScoped\<IUserService, UserService\>();

builder.Services.AddSingleton\<ICacheService, MemoryCacheService\>();

// \--- Đăng ký Authentication (JWT) \---

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    .AddJwtBearer(options \=\> { /\* cấu hình JWT \*/ });

// \--- Đăng ký CORS \---

builder.Services.AddCors(options \=\>

{

    options.AddPolicy("AllowFrontend", policy \=\>

    {

        policy.WithOrigins("http://localhost:3000") // URL frontend React/Vue

              .AllowAnyHeader()

              .AllowAnyMethod();

    });

});

var app \= builder.Build();

// \==========================================

//  PHẦN 2: MIDDLEWARE PIPELINE — Xử lý Request

// \==========================================

// THỨ TỰ RẤT QUAN TRỌNG\! Request đi qua từng middleware từ trên xuống.

// Chỉ bật Swagger trong môi trường Development

if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI();

}

app.UseHttpsRedirection();      // Chuyển HTTP → HTTPS

app.UseCors("AllowFrontend");   // Xử lý CORS

app.UseAuthentication();         // Xác thực: Ai đang gọi? (phải trước Authorization)

app.UseAuthorization();          // Phân quyền: Có được phép không?

app.MapControllers();            // Map route đến controllers

app.Run();

### Giải thích luồng hoạt động

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

---

## 4\. Controller — Xử Lý Request

### 4.1 Controller cơ bản (CRUD hoàn chỉnh)

using Microsoft.AspNetCore.Mvc;

\[ApiController\]                    // Bật tính năng API (auto validation, auto 400...)

\[Route("api/\[controller\]")\]        // Route: api/products

public class ProductsController : ControllerBase

{

    private readonly IProductService \_productService;

    private readonly ILogger\<ProductsController\> \_logger;

    // Dependency Injection qua constructor

    public ProductsController(IProductService productService, ILogger\<ProductsController\> logger)

    {

        \_productService \= productService;

        \_logger \= logger;

    }

    // \===== GET api/products \=====

    // Lấy danh sách sản phẩm (có phân trang, tìm kiếm)

    \[HttpGet\]

    \[ProducesResponseType(typeof(PagedResult\<ProductDto\>), StatusCodes.Status200OK)\]

    public async Task\<ActionResult\<PagedResult\<ProductDto\>\>\> GetAll(

        \[FromQuery\] int page \= 1,

        \[FromQuery\] int pageSize \= 10,

        \[FromQuery\] string? search \= null)

    {

        \_logger.LogInformation("Lấy danh sách sản phẩm, trang {Page}", page);

        var result \= await \_productService.GetAllAsync(page, pageSize, search);

        return Ok(result);

    }

    // \===== GET api/products/5 \=====

    // Lấy chi tiết một sản phẩm

    \[HttpGet("{id:int}")\]           // {id:int} \= route constraint, chỉ chấp nhận số

    \[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)\]

    \[ProducesResponseType(StatusCodes.Status404NotFound)\]

    public async Task\<ActionResult\<ProductDto\>\> GetById(int id)

    {

        var product \= await \_productService.GetByIdAsync(id);

        if (product is null)

            return NotFound(new { message \= \$"Không tìm thấy sản phẩm với id \= {id}" });

        return Ok(product);

    }

    // \===== POST api/products \=====

    // Tạo sản phẩm mới

    \[HttpPost\]

    \[ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)\]

    \[ProducesResponseType(StatusCodes.Status400BadRequest)\]

    public async Task\<ActionResult\<ProductDto\>\> Create(\[FromBody\] CreateProductRequest request)

    {

        // \[ApiController\] tự động validate và trả 400 nếu request không hợp lệ

        var product \= await \_productService.CreateAsync(request);

        // Trả về 201 Created \+ header Location: api/products/{id}

        return CreatedAtAction(nameof(GetById), new { id \= product.Id }, product);

    }

    // \===== PUT api/products/5 \=====

    // Cập nhật toàn bộ sản phẩm

    \[HttpPut("{id:int}")\]

    \[ProducesResponseType(StatusCodes.Status204NoContent)\]

    \[ProducesResponseType(StatusCodes.Status404NotFound)\]

    public async Task\<ActionResult\> Update(int id, \[FromBody\] UpdateProductRequest request)

    {

        var exists \= await \_productService.ExistsAsync(id);

        if (\!exists)

            return NotFound();

        await \_productService.UpdateAsync(id, request);

        return NoContent(); // 204 — cập nhật thành công, không có body

    }

    // \===== DELETE api/products/5 \=====

    // Xóa sản phẩm

    \[HttpDelete("{id:int}")\]

    \[ProducesResponseType(StatusCodes.Status204NoContent)\]

    \[ProducesResponseType(StatusCodes.Status404NotFound)\]

    public async Task\<ActionResult\> Delete(int id)

    {

        var exists \= await \_productService.ExistsAsync(id);

        if (\!exists)

            return NotFound();

        await \_productService.DeleteAsync(id);

        return NoContent();

    }

}

### 4.2 Các kiểu trả về thường dùng

return Ok(data);                  // 200 — Thành công, có dữ liệu

return NoContent();               // 204 — Thành công, không có dữ liệu

return Created(uri, data);        // 201 — Tạo mới thành công

return CreatedAtAction(...);      // 201 — Tạo mới \+ link đến resource

return BadRequest(errors);        // 400 — Request không hợp lệ

return Unauthorized();            // 401 — Chưa đăng nhập

return Forbid();                  // 403 — Không có quyền

return NotFound();                // 404 — Không tìm thấy

return Conflict();                // 409 — Xung đột (ví dụ email đã tồn tại)

return StatusCode(500, "Lỗi");   // 500 — Lỗi server (ít dùng trực tiếp)

### 4.3 Binding dữ liệu từ Request

// \[FromBody\]  — Dữ liệu từ JSON body (POST, PUT)

\[HttpPost\]

public ActionResult Create(\[FromBody\] CreateProductRequest request) { }

// \[FromQuery\] — Dữ liệu từ query string (?page=1\&search=abc)

\[HttpGet\]

public ActionResult Search(\[FromQuery\] string search, \[FromQuery\] int page \= 1\) { }

// \[FromRoute\] — Dữ liệu từ URL path (/api/products/5)

\[HttpGet("{id}")\]

public ActionResult GetById(\[FromRoute\] int id) { }

// \[FromHeader\] — Dữ liệu từ HTTP header

\[HttpGet\]

public ActionResult Get(\[FromHeader(Name \= "X-Api-Key")\] string apiKey) { }

// \[FromForm\] — Dữ liệu từ form (file upload)

\[HttpPost("upload")\]

public ActionResult Upload(\[FromForm\] IFormFile file) { }

---

## 5\. Minimal APIs — Cách Viết Gọn

Từ .NET 6+, bạn có thể viết API mà không cần Controller. Phù hợp cho API nhỏ, microservice.

var builder \= WebApplication.CreateBuilder(args);

builder.Services.AddScoped\<IProductService, ProductService\>();

var app \= builder.Build();

// Định nghĩa endpoint trực tiếp trong Program.cs

var products \= app.MapGroup("/api/products");  // Nhóm route

products.MapGet("/", async (IProductService service, int page \= 1, int pageSize \= 10\) \=\>

{

    var result \= await service.GetAllAsync(page, pageSize);

    return Results.Ok(result);

});

products.MapGet("/{id:int}", async (int id, IProductService service) \=\>

{

    var product \= await service.GetByIdAsync(id);

    return product is not null

        ? Results.Ok(product)

        : Results.NotFound();

});

products.MapPost("/", async (CreateProductRequest request, IProductService service) \=\>

{

    var product \= await service.CreateAsync(request);

    return Results.Created(\$"/api/products/{product.Id}", product);

});

products.MapPut("/{id:int}", async (int id, UpdateProductRequest request, IProductService service) \=\>

{

    await service.UpdateAsync(id, request);

    return Results.NoContent();

});

products.MapDelete("/{id:int}", async (int id, IProductService service) \=\>

{

    await service.DeleteAsync(id);

    return Results.NoContent();

});

app.Run();

> **Controller vs Minimal API:** Controller phù hợp cho dự án lớn, nhiều endpoint, cần tổ chức rõ ràng. Minimal API phù hợp cho microservice nhỏ, prototype nhanh. Cả hai đều chạy trên cùng một nền tảng ASP.NET Core.

---

## 6\. Dependency Injection (DI) — Chi Tiết

DI là pattern cốt lõi nhất trong ASP.NET Core. Hiểu DI \= hiểu ASP.NET Core.

### 6.1 Tại sao cần DI?

// ❌ KHÔNG có DI — Tightly coupled (phụ thuộc chặt)

public class ProductsController

{

    public ActionResult GetAll()

    {

        // Controller tự tạo mọi thứ → Không thể test, không thể đổi implementation

        var context \= new AppDbContext(/\* phải tự truyền options \*/);

        var logger \= new ConsoleLogger();

        var products \= context.Products.ToList();

        return Ok(products);

    }

}

// ✅ CÓ DI — Loosely coupled (phụ thuộc lỏng)

public class ProductsController

{

    private readonly IProductService \_service; // Chỉ biết interface, KHÔNG biết implementation

    public ProductsController(IProductService service) // Framework tự tiêm vào

    {

        \_service \= service;

    }

    public ActionResult GetAll()

    {

        var products \= \_service.GetAll(); // Gọi qua interface

        return Ok(products);

    }

}

### 6.2 Ba loại Lifetime

// \--- TRANSIENT: Tạo MỚI mỗi khi được yêu cầu \---

builder.Services.AddTransient\<IEmailService, EmailService\>();

// Dùng cho: Service nhẹ, không giữ state

// Ví dụ: EmailService, PdfGenerator, Validator

// \--- SCOPED: Tạo MỘT lần cho mỗi HTTP request \---

builder.Services.AddScoped\<IProductService, ProductService\>();

builder.Services.AddScoped\<IUnitOfWork, UnitOfWork\>();

// Dùng cho: Service làm việc với database, business logic

// Phổ biến nhất\! DbContext mặc định cũng là Scoped

// \--- SINGLETON: Tạo MỘT lần duy nhất cho toàn bộ ứng dụng \---

builder.Services.AddSingleton\<ICacheService, MemoryCacheService\>();

builder.Services.AddSingleton\<IConfiguration\>(builder.Configuration);

// Dùng cho: Cache, configuration, HttpClient factory

// Cẩn thận: Thread-safe bắt buộc\!

### 6.3 Đăng ký DI thực tế

// Interface

public interface IProductService

{

    Task\<List\<ProductDto\>\> GetAllAsync();

    Task\<ProductDto?\> GetByIdAsync(int id);

    Task\<ProductDto\> CreateAsync(CreateProductRequest request);

}

// Implementation

public class ProductService : IProductService

{

    private readonly AppDbContext \_context;

    private readonly ILogger\<ProductService\> \_logger;

    public ProductService(AppDbContext context, ILogger\<ProductService\> logger)

    {

        \_context \= context;   // DbContext cũng được DI tiêm vào

        \_logger \= logger;     // Logger cũng được DI tiêm vào

    }

    public async Task\<List\<ProductDto\>\> GetAllAsync()

    {

        return await \_context.Products

            .Select(p \=\> new ProductDto(p.Id, p.Name, p.Price))

            .ToListAsync();

    }

    public async Task\<ProductDto?\> GetByIdAsync(int id)

    {

        var product \= await \_context.Products.FindAsync(id);

        return product is null ? null : new ProductDto(product.Id, product.Name, product.Price);

    }

    public async Task\<ProductDto\> CreateAsync(CreateProductRequest request)

    {

        var product \= new Product { Name \= request.Name, Price \= request.Price };

        \_context.Products.Add(product);

        await \_context.SaveChangesAsync();

        \_logger.LogInformation("Tạo sản phẩm mới: {ProductName}", product.Name);

        return new ProductDto(product.Id, product.Name, product.Price);

    }

}

// Đăng ký trong Program.cs

builder.Services.AddScoped\<IProductService, ProductService\>();

---

## 7\. Entity Framework Core — Chi Tiết

### 7.1 Cài đặt

dotnet add package Microsoft.EntityFrameworkCore.SqlServer    \# SQL Server

dotnet add package Microsoft.EntityFrameworkCore.Design       \# Cho migrations

dotnet tool install \--global dotnet-ef                        \# CLI tool

\# Hoặc dùng database khác

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL      \# PostgreSQL

dotnet add package Microsoft.EntityFrameworkCore.Sqlite       \# SQLite (dev/test)

### 7.2 Định nghĩa Entities (Models)

public class Product

{

    public int Id { get; set; }                  // Primary Key (convention: Id hoặc ProductId)

    public string Name { get; set; } \= string.Empty;

    public string? Description { get; set; }     // Nullable — không bắt buộc

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public bool IsActive { get; set; } \= true;

    public DateTime CreatedAt { get; set; } \= DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // \--- Navigation Properties (quan hệ) \---

    public int CategoryId { get; set; }          // Foreign Key

    public Category Category { get; set; } \= null\!;  // Quan hệ N-1

    public List\<OrderItem\> OrderItems { get; set; } \= \[\]; // Quan hệ 1-N

    public List\<Tag\> Tags { get; set; } \= \[\];             // Quan hệ N-N

}

public class Category

{

    public int Id { get; set; }

    public string Name { get; set; } \= string.Empty;

    public string? Description { get; set; }

    public List\<Product\> Products { get; set; } \= \[\];     // Quan hệ 1-N

}

public class Order

{

    public int Id { get; set; }

    public DateTime OrderDate { get; set; } \= DateTime.UtcNow;

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; } \= OrderStatus.Pending;

    public int UserId { get; set; }

    public User User { get; set; } \= null\!;

    public List\<OrderItem\> Items { get; set; } \= \[\];

}

public class OrderItem

{

    public int Id { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice \=\> Quantity \* UnitPrice; // Computed property

    public int OrderId { get; set; }

    public Order Order { get; set; } \= null\!;

    public int ProductId { get; set; }

    public Product Product { get; set; } \= null\!;

}

public enum OrderStatus

{

    Pending,

    Confirmed,

    Shipping,

    Delivered,

    Cancelled

}

### 7.3 DbContext

public class AppDbContext : DbContext

{

    public AppDbContext(DbContextOptions\<AppDbContext\> options) : base(options) { }

    // Mỗi DbSet tương ứng với một bảng trong database

    public DbSet\<Product\> Products { get; set; }

    public DbSet\<Category\> Categories { get; set; }

    public DbSet\<Order\> Orders { get; set; }

    public DbSet\<OrderItem\> OrderItems { get; set; }

    public DbSet\<User\> Users { get; set; }

    // Fluent API — cấu hình chi tiết database schema

    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {

        // \--- Product \---

        modelBuilder.Entity\<Product\>(entity \=\>

        {

            entity.HasKey(e \=\> e.Id);

            entity.Property(e \=\> e.Name)

                  .IsRequired()

                  .HasMaxLength(200);

            entity.Property(e \=\> e.Price)

                  .HasPrecision(18, 2);       // decimal(18,2) cho tiền

            entity.HasIndex(e \=\> e.Name);     // Tạo index để tìm kiếm nhanh

            // Quan hệ N-1 với Category

            entity.HasOne(e \=\> e.Category)

                  .WithMany(c \=\> c.Products)

                  .HasForeignKey(e \=\> e.CategoryId)

                  .OnDelete(DeleteBehavior.Restrict); // Không cho xóa category nếu còn product

        });

        // \--- Order \---

        modelBuilder.Entity\<Order\>(entity \=\>

        {

            entity.Property(e \=\> e.TotalAmount).HasPrecision(18, 2);

            entity.Property(e \=\> e.Status)

                  .HasConversion\<string\>()    // Lưu enum dạng string trong DB

                  .HasMaxLength(20);

        });

        // \--- Quan hệ N-N (Product ↔ Tag) \---

        modelBuilder.Entity\<Product\>()

            .HasMany(p \=\> p.Tags)

            .WithMany(t \=\> t.Products)

            .UsingEntity(j \=\> j.ToTable("ProductTags")); // Bảng trung gian

        // \--- Seed data (dữ liệu mẫu) \---

        modelBuilder.Entity\<Category\>().HasData(

            new Category { Id \= 1, Name \= "Điện thoại" },

            new Category { Id \= 2, Name \= "Laptop" },

            new Category { Id \= 3, Name \= "Phụ kiện" }

        );

    }

}

### 7.4 Migration

\# Tạo migration đầu tiên

dotnet ef migrations add InitialCreate

\# Xem SQL sẽ được chạy (không thực thi)

dotnet ef migrations script

\# Áp dụng migration vào database

dotnet ef database update

\# Khi thay đổi model, tạo migration mới

dotnet ef migrations add AddProductStock

\# Rollback migration

dotnet ef database update PreviousMigrationName

\# Xóa migration cuối (nếu chưa apply)

dotnet ef migrations remove

### 7.5 Truy vấn với LINQ (Các pattern phổ biến)

public class ProductService : IProductService

{

    private readonly AppDbContext \_context;

    // \--- Lấy danh sách có phân trang \+ tìm kiếm \+ lọc \---

    public async Task\<PagedResult\<ProductDto\>\> GetAllAsync(

        int page, int pageSize, string? search, int? categoryId)

    {

        var query \= \_context.Products

            .Include(p \=\> p.Category)     // JOIN với Category

            .AsQueryable();

        // Lọc theo điều kiện (dynamic filter)

        if (\!string.IsNullOrWhiteSpace(search))

            query \= query.Where(p \=\> p.Name.Contains(search));

        if (categoryId.HasValue)

            query \= query.Where(p \=\> p.CategoryId \== categoryId.Value);

        // Đếm tổng (cho phân trang)

        var totalCount \= await query.CountAsync();

        // Lấy dữ liệu theo trang

        var items \= await query

            .OrderByDescending(p \=\> p.CreatedAt)

            .Skip((page \- 1\) \* pageSize)

            .Take(pageSize)

            .Select(p \=\> new ProductDto         // Chỉ lấy các trường cần thiết

            {

                Id \= p.Id,

                Name \= p.Name,

                Price \= p.Price,

                CategoryName \= p.Category.Name

            })

            .ToListAsync();

        return new PagedResult\<ProductDto\>

        {

            Items \= items,

            TotalCount \= totalCount,

            Page \= page,

            PageSize \= pageSize,

            TotalPages \= (int)Math.Ceiling(totalCount / (double)pageSize)

        };

    }

    // \--- Lấy chi tiết với nhiều quan hệ \---

    public async Task\<ProductDetailDto?\> GetByIdAsync(int id)

    {

        return await \_context.Products

            .Include(p \=\> p.Category)

            .Include(p \=\> p.Tags)

            .Include(p \=\> p.OrderItems)

                .ThenInclude(oi \=\> oi.Order)   // Include lồng nhau

            .Where(p \=\> p.Id \== id)

            .Select(p \=\> new ProductDetailDto

            {

                Id \= p.Id,

                Name \= p.Name,

                Price \= p.Price,

                Category \= p.Category.Name,

                Tags \= p.Tags.Select(t \=\> t.Name).ToList(),

                TotalOrders \= p.OrderItems.Count,

                TotalRevenue \= p.OrderItems.Sum(oi \=\> oi.TotalPrice)

            })

            .FirstOrDefaultAsync();

    }

    // \--- Thống kê / Aggregate \---

    public async Task\<DashboardDto\> GetDashboardAsync()

    {

        return new DashboardDto

        {

            TotalProducts \= await \_context.Products.CountAsync(),

            TotalRevenue \= await \_context.OrderItems.SumAsync(oi \=\> oi.TotalPrice),

            TopCategories \= await \_context.Categories

                .Select(c \=\> new CategoryStatsDto

                {

                    Name \= c.Name,

                    ProductCount \= c.Products.Count,

                    Revenue \= c.Products

                        .SelectMany(p \=\> p.OrderItems)

                        .Sum(oi \=\> oi.TotalPrice)

                })

                .OrderByDescending(c \=\> c.Revenue)

                .Take(5)

                .ToListAsync()

        };

    }

}

---

## 8\. DTOs và Request/Response Models

// \--- Response DTOs \---

public record ProductDto

{

    public int Id { get; init; }

    public string Name { get; init; } \= string.Empty;

    public decimal Price { get; init; }

    public string CategoryName { get; init; } \= string.Empty;

}

public record PagedResult\<T\>

{

    public List\<T\> Items { get; init; } \= \[\];

    public int TotalCount { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalPages { get; init; }

    public bool HasPrevious \=\> Page \> 1;

    public bool HasNext \=\> Page \< TotalPages;

}

// \--- Request DTOs \---

public record CreateProductRequest

{

    \[Required(ErrorMessage \= "Tên sản phẩm không được để trống")\]

    \[StringLength(200, MinimumLength \= 2)\]

    public string Name { get; init; } \= string.Empty;

    \[StringLength(1000)\]

    public string? Description { get; init; }

    \[Required\]

    \[Range(0.01, 999\_999\_999, ErrorMessage \= "Giá phải lớn hơn 0")\]

    public decimal Price { get; init; }

    \[Range(0, int.MaxValue)\]

    public int Stock { get; init; }

    \[Required\]

    public int CategoryId { get; init; }

}

public record UpdateProductRequest

{

    \[Required\]

    \[StringLength(200, MinimumLength \= 2)\]

    public string Name { get; init; } \= string.Empty;

    public string? Description { get; init; }

    \[Range(0.01, 999\_999\_999)\]

    public decimal Price { get; init; }

    \[Range(0, int.MaxValue)\]

    public int Stock { get; init; }

}

// \--- API Error Response \---

public record ApiError(string Message, Dictionary\<string, string\[\]\>? Errors \= null);

---

## 9\. Middleware — Viết Middleware Tùy Chỉnh

### 9.1 Exception Handling Middleware (Bắt buộc có)

public class GlobalExceptionMiddleware

{

    private readonly RequestDelegate \_next;

    private readonly ILogger\<GlobalExceptionMiddleware\> \_logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger\<GlobalExceptionMiddleware\> logger)

    {

        \_next \= next;

        \_logger \= logger;

    }

    public async Task InvokeAsync(HttpContext context)

    {

        try

        {

            await \_next(context); // Chuyển tiếp cho middleware tiếp theo

        }

        catch (NotFoundException ex)

        {

            \_logger.LogWarning(ex, "Resource not found");

            context.Response.StatusCode \= 404;

            await context.Response.WriteAsJsonAsync(new ApiError(ex.Message));

        }

        catch (ValidationException ex)

        {

            \_logger.LogWarning(ex, "Validation failed");

            context.Response.StatusCode \= 400;

            await context.Response.WriteAsJsonAsync(new ApiError(ex.Message));

        }

        catch (Exception ex)

        {

            \_logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            context.Response.StatusCode \= 500;

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

app.UseMiddleware\<GlobalExceptionMiddleware\>();

### 9.2 Logging Middleware

public class RequestLoggingMiddleware

{

    private readonly RequestDelegate \_next;

    private readonly ILogger\<RequestLoggingMiddleware\> \_logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger\<RequestLoggingMiddleware\> logger)

    {

        \_next \= next;

        \_logger \= logger;

    }

    public async Task InvokeAsync(HttpContext context)

    {

        var stopwatch \= Stopwatch.StartNew();

        var method \= context.Request.Method;

        var path \= context.Request.Path;

        \_logger.LogInformation("→ {Method} {Path}", method, path);

        await \_next(context);

        stopwatch.Stop();

        var statusCode \= context.Response.StatusCode;

        \_logger.LogInformation("← {Method} {Path} → {StatusCode} ({ElapsedMs}ms)",

            method, path, statusCode, stopwatch.ElapsedMilliseconds);

    }

}

---

## 10\. Authentication & Authorization

### 10.1 JWT Authentication

// \--- appsettings.json \---

{

    "JwtSettings": {

        "SecretKey": "your-secret-key-at-least-32-characters-long\!",

        "Issuer": "MyApp",

        "Audience": "MyApp",

        "ExpirationInMinutes": 60

    }

}

// \--- JwtSettings.cs \---

public class JwtSettings

{

    public string SecretKey { get; set; } \= string.Empty;

    public string Issuer { get; set; } \= string.Empty;

    public string Audience { get; set; } \= string.Empty;

    public int ExpirationInMinutes { get; set; }

}

// \--- Đăng ký trong Program.cs \---

var jwtSettings \= builder.Configuration.GetSection("JwtSettings").Get\<JwtSettings\>()\!;

builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    .AddJwtBearer(options \=\>

    {

        options.TokenValidationParameters \= new TokenValidationParameters

        {

            ValidateIssuer \= true,

            ValidateAudience \= true,

            ValidateLifetime \= true,

            ValidateIssuerSigningKey \= true,

            ValidIssuer \= jwtSettings.Issuer,

            ValidAudience \= jwtSettings.Audience,

            IssuerSigningKey \= new SymmetricSecurityKey(

                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))

        };

    });

// \--- TokenService.cs — Tạo JWT token \---

public class TokenService

{

    private readonly JwtSettings \_jwtSettings;

    public TokenService(JwtSettings jwtSettings)

    {

        \_jwtSettings \= jwtSettings;

    }

    public string GenerateToken(User user)

    {

        var claims \= new List\<Claim\>

        {

            new(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new(ClaimTypes.Email, user.Email),

            new(ClaimTypes.Name, user.FullName),

            new(ClaimTypes.Role, user.Role)    // "Admin", "User", etc.

        };

        var key \= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(\_jwtSettings.SecretKey));

        var credentials \= new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token \= new JwtSecurityToken(

            issuer: \_jwtSettings.Issuer,

            audience: \_jwtSettings.Audience,

            claims: claims,

            expires: DateTime.UtcNow.AddMinutes(\_jwtSettings.ExpirationInMinutes),

            signingCredentials: credentials

        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

}

### 10.2 Auth Controller

\[ApiController\]

\[Route("api/\[controller\]")\]

public class AuthController : ControllerBase

{

    private readonly IAuthService \_authService;

    public AuthController(IAuthService authService) \=\> \_authService \= authService;

    \[HttpPost("register")\]

    public async Task\<ActionResult\<AuthResponse\>\> Register(RegisterRequest request)

    {

        var result \= await \_authService.RegisterAsync(request);

        return Ok(result);

    }

    \[HttpPost("login")\]

    public async Task\<ActionResult\<AuthResponse\>\> Login(LoginRequest request)

    {

        var result \= await \_authService.LoginAsync(request);

        if (result is null)

            return Unauthorized(new ApiError("Email hoặc mật khẩu không đúng"));

        return Ok(result);

    }

    // Endpoint yêu cầu đăng nhập

    \[Authorize\]

    \[HttpGet("me")\]

    public ActionResult\<UserDto\> GetCurrentUser()

    {

        var userId \= User.FindFirstValue(ClaimTypes.NameIdentifier);

        var email \= User.FindFirstValue(ClaimTypes.Email);

        return Ok(new { userId, email });

    }

    // Endpoint chỉ dành cho Admin

    \[Authorize(Roles \= "Admin")\]

    \[HttpGet("admin/stats")\]

    public ActionResult GetAdminStats()

    {

        return Ok(new { message \= "Chỉ Admin mới thấy được" });

    }

}

// Request/Response models

public record RegisterRequest(string FullName, string Email, string Password);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, string FullName, string Email, string Role);

---

## 11\. Configuration (Cấu hình)

### 11.1 appsettings.json

{

    "ConnectionStrings": {

        "DefaultConnection": "Server=localhost;Database=MyApp;Trusted\_Connection=true;"

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

### 11.2 Đọc configuration

// \--- Cách 1: Options Pattern (khuyến khích) \---

public class EmailSettings

{

    public string SmtpHost { get; set; } \= string.Empty;

    public int SmtpPort { get; set; }

    public string SenderEmail { get; set; } \= string.Empty;

}

// Đăng ký

builder.Services.Configure\<EmailSettings\>(

    builder.Configuration.GetSection("EmailSettings"));

// Sử dụng

public class EmailService

{

    private readonly EmailSettings \_settings;

    public EmailService(IOptions\<EmailSettings\> options)

    {

        \_settings \= options.Value; // Lấy giá trị từ configuration

    }

}

// \--- Cách 2: Đọc trực tiếp \---

var secretKey \= builder.Configuration\["JwtSettings:SecretKey"\];

var connectionString \= builder.Configuration.GetConnectionString("DefaultConnection");

// \--- Environment-specific config \---

// appsettings.Development.json    → Dùng khi chạy local

// appsettings.Production.json     → Dùng khi deploy

// Environment variables           → Override tất cả (ưu tiên cao nhất)

---

## 12\. Validation Nâng Cao Với FluentValidation

dotnet add package FluentValidation.AspNetCore

public class CreateProductValidator : AbstractValidator\<CreateProductRequest\>

{

    private readonly AppDbContext \_context;

    public CreateProductValidator(AppDbContext context) // DI hoạt động trong validator

    {

        \_context \= context;

        RuleFor(x \=\> x.Name)

            .NotEmpty().WithMessage("Tên sản phẩm không được để trống")

            .MaximumLength(200).WithMessage("Tên tối đa 200 ký tự")

            .MustAsync(BeUniqueName).WithMessage("Tên sản phẩm đã tồn tại");

        RuleFor(x \=\> x.Price)

            .GreaterThan(0).WithMessage("Giá phải lớn hơn 0")

            .LessThan(1\_000\_000\_000).WithMessage("Giá không hợp lệ");

        RuleFor(x \=\> x.CategoryId)

            .MustAsync(CategoryExists).WithMessage("Danh mục không tồn tại");

        RuleFor(x \=\> x.Stock)

            .GreaterThanOrEqualTo(0).WithMessage("Số lượng không được âm");

    }

    private async Task\<bool\> BeUniqueName(string name, CancellationToken ct)

    {

        return \!await \_context.Products.AnyAsync(p \=\> p.Name \== name, ct);

    }

    private async Task\<bool\> CategoryExists(int categoryId, CancellationToken ct)

    {

        return await \_context.Categories.AnyAsync(c \=\> c.Id \== categoryId, ct);

    }

}

// Đăng ký tất cả validator trong assembly

builder.Services.AddValidatorsFromAssemblyContaining\<CreateProductValidator\>();

---

## 13\. CORS (Cross-Origin Resource Sharing)

CORS cho phép frontend (React, Vue...) ở domain khác gọi API của bạn.

// Program.cs

builder.Services.AddCors(options \=\>

{

    // Policy cho Development

    options.AddPolicy("Development", policy \=\>

    {

        policy.WithOrigins(

                "http://localhost:3000",      // React dev server

                "http://localhost:5173")      // Vite dev server

              .AllowAnyHeader()

              .AllowAnyMethod()

              .AllowCredentials();            // Cho phép gửi cookie/token

    });

    // Policy cho Production

    options.AddPolicy("Production", policy \=\>

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

---

## 14\. Logging

// ILogger được inject tự động, không cần đăng ký thêm

public class ProductService

{

    private readonly ILogger\<ProductService\> \_logger;

    public ProductService(ILogger\<ProductService\> logger) \=\> \_logger \= logger;

    public async Task\<ProductDto\> CreateAsync(CreateProductRequest request)

    {

        \_logger.LogInformation("Tạo sản phẩm mới: {ProductName}, giá: {Price}",

            request.Name, request.Price);

        try

        {

            // ... tạo sản phẩm

            \_logger.LogInformation("Tạo thành công sản phẩm ID: {ProductId}", product.Id);

        }

        catch (Exception ex)

        {

            \_logger.LogError(ex, "Lỗi khi tạo sản phẩm: {ProductName}", request.Name);

            throw;

        }

    }

}

// Các log level (từ thấp đến cao):

// Trace → Debug → Information → Warning → Error → Critical

---

## 15\. File Upload

\[HttpPost("upload")\]

\[RequestSizeLimit(10 \* 1024 \* 1024)\]  // Giới hạn 10MB

public async Task\<ActionResult\<string\>\> Upload(\[FromForm\] IFormFile file)

{

    if (file.Length \== 0\)

        return BadRequest("File trống");

    // Kiểm tra loại file

    var allowedExtensions \= new\[\] { ".jpg", ".jpeg", ".png", ".pdf" };

    var extension \= Path.GetExtension(file.FileName).ToLowerInvariant();

    if (\!allowedExtensions.Contains(extension))

        return BadRequest("Loại file không được hỗ trợ");

    // Tạo tên file unique

    var fileName \= \$"{Guid.NewGuid()}{extension}";

    var filePath \= Path.Combine("wwwroot", "uploads", fileName);

    // Lưu file

    Directory.CreateDirectory(Path.GetDirectoryName(filePath)\!);

    await using var stream \= new FileStream(filePath, FileMode.Create);

    await file.CopyToAsync(stream);

    var url \= \$"/uploads/{fileName}";

    return Ok(new { url });

}

// Upload nhiều file

\[HttpPost("upload-multiple")\]

public async Task\<ActionResult\> UploadMultiple(\[FromForm\] List\<IFormFile\> files)

{

    var urls \= new List\<string\>();

    foreach (var file in files)

    {

        // ... xử lý từng file

    }

    return Ok(new { urls });

}

---

## 16\. Background Services

// Service chạy nền — ví dụ: gửi email hàng đợi

public class EmailBackgroundService : BackgroundService

{

    private readonly IServiceProvider \_serviceProvider;

    private readonly ILogger\<EmailBackgroundService\> \_logger;

    public EmailBackgroundService(IServiceProvider serviceProvider,

        ILogger\<EmailBackgroundService\> logger)

    {

        \_serviceProvider \= serviceProvider;

        \_logger \= logger;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)

    {

        \_logger.LogInformation("Email Background Service đang chạy");

        while (\!stoppingToken.IsCancellationRequested)

        {

            // Tạo scope mới để lấy Scoped services (DbContext)

            using var scope \= \_serviceProvider.CreateScope();

            var emailQueue \= scope.ServiceProvider.GetRequiredService\<IEmailQueue\>();

            var pendingEmails \= await emailQueue.GetPendingAsync();

            foreach (var email in pendingEmails)

            {

                await emailQueue.SendAsync(email);

            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Chờ 30s

        }

    }

}

// Đăng ký

builder.Services.AddHostedService\<EmailBackgroundService\>();

---

## 17\. Cấu Trúc Project Thực Tế (Clean Architecture)

Solution/

│

├── src/

│   ├── MyApp.Api/                      ← Presentation Layer

│   │   ├── Controllers/

│   │   │   ├── AuthController.cs

│   │   │   ├── ProductsController.cs

│   │   │   └── OrdersController.cs

│   │   ├── Middlewares/

│   │   │   ├── GlobalExceptionMiddleware.cs

│   │   │   └── RequestLoggingMiddleware.cs

│   │   ├── Filters/

│   │   ├── appsettings.json

│   │   └── Program.cs

│   │

│   ├── MyApp.Application/              ← Business Logic Layer

│   │   ├── Interfaces/                 \# Định nghĩa interface (IProductService, IEmailService...)

│   │   ├── Services/                   \# Implement business logic

│   │   ├── DTOs/                       \# Request/Response models

│   │   ├── Validators/                 \# FluentValidation validators

│   │   └── Mappings/                   \# AutoMapper profiles

│   │

│   ├── MyApp.Domain/                   ← Domain Layer (trong cùng, không phụ thuộc gì)

│   │   ├── Entities/                   \# Product, Order, User...

│   │   ├── Enums/                      \# OrderStatus, UserRole...

│   │   └── Exceptions/                 \# Domain exceptions

│   │

│   └── MyApp.Infrastructure/           ← Infrastructure Layer

│       ├── Data/

│       │   ├── AppDbContext.cs

│       │   ├── Configurations/         \# Fluent API configurations

│       │   └── Migrations/

│       ├── Repositories/               \# Data access

│       └── Services/                   \# Email, Storage, Cache...

│

└── tests/

    ├── MyApp.UnitTests/

    └── MyApp.IntegrationTests/

### Quy tắc phụ thuộc

Api → Application → Domain ← Infrastructure

                       ↑

                  Infrastructure

- **Domain** không phụ thuộc vào bất cứ thứ gì.  
- **Application** chỉ phụ thuộc Domain.  
- **Infrastructure** phụ thuộc Domain (implement interfaces).  
- **Api** phụ thuộc Application và đăng ký Infrastructure qua DI.

---

## 18\. Deploy Cơ Bản Với Docker

\# \--- Dockerfile \---

\# Stage 1: Build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY \["MyApp.Api/MyApp.Api.csproj", "MyApp.Api/"\]

COPY \["MyApp.Application/MyApp.Application.csproj", "MyApp.Application/"\]

COPY \["MyApp.Domain/MyApp.Domain.csproj", "MyApp.Domain/"\]

COPY \["MyApp.Infrastructure/MyApp.Infrastructure.csproj", "MyApp.Infrastructure/"\]

RUN dotnet restore "MyApp.Api/MyApp.Api.csproj"

COPY . .

RUN dotnet publish "MyApp.Api/MyApp.Api.csproj" \-c Release \-o /app/publish

\# Stage 2: Runtime (image nhẹ hơn nhiều)

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY \--from=build /app/publish .

EXPOSE 8080

ENTRYPOINT \["dotnet", "MyApp.Api.dll"\]

\# Build và chạy

docker build \-t myapp .

docker run \-p 8080:8080 myapp

---

## Tóm Tắt Lộ Trình

| Giai đoạn | Chủ đề | Thời gian (ước tính) |
| :---- | :---- | :---- |
| 1 | Program.cs, Controller, Routing, HTTP methods | 1-2 tuần |
| 2 | Entity Framework Core, LINQ, Migration | 1-2 tuần |
| 3 | DI, Services, DTOs, Validation | 1 tuần |
| 4 | Middleware, Error Handling, Logging | 1 tuần |
| 5 | Authentication (JWT), Authorization | 1 tuần |
| 6 | CORS, File Upload, Configuration | 3-5 ngày |
| 7 | Clean Architecture, Project Structure | 1 tuần |
| 8 | Testing (Unit \+ Integration) | 1 tuần |
| 9 | Docker, CI/CD, Deploy | 1 tuần |

> **Mẹo học:** Đừng chỉ đọc — hãy code theo. Tạo một project thực tế (quản lý sản phẩm, blog, todo app) và áp dụng từng kiến thức vào. Đó là cách học nhanh nhất.  
