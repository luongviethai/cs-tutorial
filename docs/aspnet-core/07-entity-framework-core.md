# 7. Entity Framework Core — Chi Tiết

## 7.1 Cài đặt

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer    # SQL Server
dotnet add package Microsoft.EntityFrameworkCore.Design       # Cho migrations
dotnet tool install --global dotnet-ef                        # CLI tool

# Hoặc dùng database khác
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL      # PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Sqlite       # SQLite (dev/test)
```

## 7.2 Định nghĩa Entities (Models)

```csharp
public class Product
{
    public int Id { get; set; }                  // Primary Key (convention: Id hoặc ProductId)
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }     // Nullable — không bắt buộc
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // --- Navigation Properties (quan hệ) ---
    public int CategoryId { get; set; }                   // Foreign Key
    public Category Category { get; set; } = null!;       // Quan hệ N-1
    public List<OrderItem> OrderItems { get; set; } = []; // Quan hệ 1-N
    public List<Tag> Tags { get; set; } = [];             // Quan hệ N-N
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Product> Products { get; set; } = [];     // Quan hệ 1-N
}

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public List<OrderItem> Items { get; set; } = [];
}

public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice; // Computed property

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipping,
    Delivered,
    Cancelled
}
```

## 7.3 DbContext

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Mỗi DbSet tương ứng với một bảng trong database
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<User> Users { get; set; }

    // Fluent API — cấu hình chi tiết database schema
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- Product ---
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.Price)
                  .HasPrecision(18, 2);       // decimal(18,2) cho tiền

            entity.HasIndex(e => e.Name);     // Tạo index để tìm kiếm nhanh

            // Quan hệ N-1 với Category
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict); // Không cho xóa category nếu còn product
        });

        // --- Order ---
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.Status)
                  .HasConversion<string>()    // Lưu enum dạng string trong DB
                  .HasMaxLength(20);
        });

        // --- Quan hệ N-N (Product ↔ Tag) ---
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.Products)
            .UsingEntity(j => j.ToTable("ProductTags")); // Bảng trung gian

        // --- Seed data (dữ liệu mẫu) ---
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Điện thoại" },
            new Category { Id = 2, Name = "Laptop" },
            new Category { Id = 3, Name = "Phụ kiện" }
        );
    }
}
```

## 7.4 Migration

```bash
# Tạo migration đầu tiên
dotnet ef migrations add InitialCreate

# Xem SQL sẽ được chạy (không thực thi)
dotnet ef migrations script

# Áp dụng migration vào database
dotnet ef database update

# Khi thay đổi model, tạo migration mới
dotnet ef migrations add AddProductStock

# Rollback migration
dotnet ef database update PreviousMigrationName

# Xóa migration cuối (nếu chưa apply)
dotnet ef migrations remove
```

## 7.5 Truy vấn với LINQ (Các pattern phổ biến)

```csharp
public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    // --- Lấy danh sách có phân trang + tìm kiếm + lọc ---
    public async Task<PagedResult<ProductDto>> GetAllAsync(
        int page, int pageSize, string? search, int? categoryId)
    {
        var query = _context.Products
            .Include(p => p.Category)     // JOIN với Category
            .AsQueryable();

        // Lọc theo điều kiện (dynamic filter)
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        // Đếm tổng (cho phân trang)
        var totalCount = await query.CountAsync();

        // Lấy dữ liệu theo trang
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto         // Chỉ lấy các trường cần thiết
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name
            })
            .ToListAsync();

        return new PagedResult<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    // --- Lấy chi tiết với nhiều quan hệ ---
    public async Task<ProductDetailDto?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Tags)
            .Include(p => p.OrderItems)
                .ThenInclude(oi => oi.Order)   // Include lồng nhau
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Category = p.Category.Name,
                Tags = p.Tags.Select(t => t.Name).ToList(),
                TotalOrders = p.OrderItems.Count,
                TotalRevenue = p.OrderItems.Sum(oi => oi.TotalPrice)
            })
            .FirstOrDefaultAsync();
    }

    // --- Thống kê / Aggregate ---
    public async Task<DashboardDto> GetDashboardAsync()
    {
        return new DashboardDto
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalRevenue = await _context.OrderItems.SumAsync(oi => oi.TotalPrice),
            TopCategories = await _context.Categories
                .Select(c => new CategoryStatsDto
                {
                    Name = c.Name,
                    ProductCount = c.Products.Count,
                    Revenue = c.Products
                        .SelectMany(p => p.OrderItems)
                        .Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(c => c.Revenue)
                .Take(5)
                .ToListAsync()
        };
    }
}
```

---

[← 6. Dependency Injection](06-dependency-injection.md) · [Mục lục](README.md) · [8. DTOs →](08-dtos.md)
