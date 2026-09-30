# 17. Cấu Trúc Project Thực Tế (Clean Architecture)

```text
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
│   │   ├── Interfaces/                 # Định nghĩa interface (IProductService, IEmailService...)
│   │   ├── Services/                   # Implement business logic
│   │   ├── DTOs/                       # Request/Response models
│   │   ├── Validators/                 # FluentValidation validators
│   │   └── Mappings/                   # AutoMapper profiles
│   │
│   ├── MyApp.Domain/                   ← Domain Layer (trong cùng, không phụ thuộc gì)
│   │   ├── Entities/                   # Product, Order, User...
│   │   ├── Enums/                      # OrderStatus, UserRole...
│   │   └── Exceptions/                 # Domain exceptions
│   │
│   └── MyApp.Infrastructure/           ← Infrastructure Layer
│       ├── Data/
│       │   ├── AppDbContext.cs
│       │   ├── Configurations/         # Fluent API configurations
│       │   └── Migrations/
│       ├── Repositories/               # Data access
│       └── Services/                   # Email, Storage, Cache...
│
└── tests/
    ├── MyApp.UnitTests/
    └── MyApp.IntegrationTests/
```

## Quy tắc phụ thuộc

```text
Api → Application → Domain ← Infrastructure
                       ↑
                  Infrastructure
```

- **Domain** không phụ thuộc vào bất cứ thứ gì.
- **Application** chỉ phụ thuộc Domain.
- **Infrastructure** phụ thuộc Domain (implement interfaces).
- **Api** phụ thuộc Application và đăng ký Infrastructure qua DI.

---

[← 16. Background Services](16-background-services.md) · [Mục lục](README.md) · [18. Docker →](18-docker.md)
