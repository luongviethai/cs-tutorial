# 12. Validation Nâng Cao Với FluentValidation

```bash
dotnet add package FluentValidation.AspNetCore
```

```csharp
public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    private readonly AppDbContext _context;

    public CreateProductValidator(AppDbContext context) // DI hoạt động trong validator
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
            .MaximumLength(200).WithMessage("Tên tối đa 200 ký tự")
            .MustAsync(BeUniqueName).WithMessage("Tên sản phẩm đã tồn tại");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Giá phải lớn hơn 0")
            .LessThan(1_000_000_000).WithMessage("Giá không hợp lệ");

        RuleFor(x => x.CategoryId)
            .MustAsync(CategoryExists).WithMessage("Danh mục không tồn tại");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Số lượng không được âm");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken ct)
    {
        return !await _context.Products.AnyAsync(p => p.Name == name, ct);
    }

    private async Task<bool> CategoryExists(int categoryId, CancellationToken ct)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId, ct);
    }
}

// Đăng ký tất cả validator trong assembly
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
```

---

[← 11. Configuration](11-configuration.md) · [Mục lục](README.md) · [13. CORS →](13-cors.md)
