# 14. Logging

```csharp
// ILogger được inject tự động, không cần đăng ký thêm
public class ProductService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request)
    {
        _logger.LogInformation("Tạo sản phẩm mới: {ProductName}, giá: {Price}",
            request.Name, request.Price);

        try
        {
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                CategoryId = request.CategoryId
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tạo thành công sản phẩm ID: {ProductId}", product.Id);

            return new ProductDto { Id = product.Id, Name = product.Name, Price = product.Price };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo sản phẩm: {ProductName}", request.Name);
            throw;
        }
    }
}

// Các log level (từ thấp đến cao):
// Trace → Debug → Information → Warning → Error → Critical
```

---

[← 13. CORS](13-cors.md) · [Mục lục](README.md) · [15. File Upload →](15-file-upload.md)
