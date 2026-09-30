# 8. DTOs và Request/Response Models

```csharp
// --- Response DTOs ---
public record ProductDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string CategoryName { get; init; } = string.Empty;
}

public record PagedResult<T>
{
    public List<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}

// --- Request DTOs ---
public record CreateProductRequest
{
    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; init; }

    [Required]
    [Range(0.01, 999_999_999, ErrorMessage = "Giá phải lớn hơn 0")]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    public int Stock { get; init; }

    [Required]
    public int CategoryId { get; init; }
}

public record UpdateProductRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    [Range(0.01, 999_999_999)]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    public int Stock { get; init; }
}

// --- API Error Response ---
public record ApiError(string Message, Dictionary<string, string[]>? Errors = null);
```

---

[← 7. Entity Framework Core](07-entity-framework-core.md) · [Mục lục](README.md) · [9. Middleware →](09-middleware.md)
