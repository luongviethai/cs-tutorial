# Hướng Dẫn ASP.NET Core — Xây Dựng Web API Từ Zero

> Bộ tài liệu được tách từ [aspnet-core-huong-dan.md](../aspnet-core-huong-dan.md) thành từng bài nhỏ.
> Đánh dấu `[x]` vào ô khi học xong một bài để theo dõi tiến độ.

## Mục lục

- [ ] [1. ASP.NET Core Là Gì?](01-aspnet-core-la-gi.md)
- [ ] [2. Cài Đặt Và Tạo Project Đầu Tiên](02-cai-dat-va-tao-project.md)
- [ ] [3. Program.cs — Trái Tim Của Ứng Dụng](03-program-cs.md)
- [ ] [4. Controller — Xử Lý Request](04-controller.md)
- [ ] [5. Minimal APIs — Cách Viết Gọn](05-minimal-apis.md)
- [ ] [6. Dependency Injection (DI) — Chi Tiết](06-dependency-injection.md)
- [ ] [7. Entity Framework Core — Chi Tiết](07-entity-framework-core.md)
- [ ] [8. DTOs và Request/Response Models](08-dtos.md)
- [ ] [9. Middleware — Viết Middleware Tùy Chỉnh](09-middleware.md)
- [ ] [10. Authentication & Authorization](10-authentication-authorization.md)
- [ ] [11. Configuration (Cấu hình)](11-configuration.md)
- [ ] [12. Validation Nâng Cao Với FluentValidation](12-fluent-validation.md)
- [ ] [13. CORS (Cross-Origin Resource Sharing)](13-cors.md)
- [ ] [14. Logging](14-logging.md)
- [ ] [15. File Upload](15-file-upload.md)
- [ ] [16. Background Services](16-background-services.md)
- [ ] [17. Cấu Trúc Project Thực Tế (Clean Architecture)](17-clean-architecture.md)
- [ ] [18. Deploy Cơ Bản Với Docker](18-docker.md)

## Tóm Tắt Lộ Trình

| Giai đoạn | Chủ đề | Thời gian (ước tính) | Bài liên quan |
| :---- | :---- | :---- | :---- |
| 1 | Program.cs, Controller, Routing, HTTP methods | 1-2 tuần | [1](01-aspnet-core-la-gi.md), [2](02-cai-dat-va-tao-project.md), [3](03-program-cs.md), [4](04-controller.md), [5](05-minimal-apis.md) |
| 2 | Entity Framework Core, LINQ, Migration | 1-2 tuần | [7](07-entity-framework-core.md) |
| 3 | DI, Services, DTOs, Validation | 1 tuần | [6](06-dependency-injection.md), [8](08-dtos.md), [12](12-fluent-validation.md) |
| 4 | Middleware, Error Handling, Logging | 1 tuần | [9](09-middleware.md), [14](14-logging.md) |
| 5 | Authentication (JWT), Authorization | 1 tuần | [10](10-authentication-authorization.md) |
| 6 | CORS, File Upload, Configuration | 3-5 ngày | [11](11-configuration.md), [13](13-cors.md), [15](15-file-upload.md) |
| 7 | Clean Architecture, Project Structure | 1 tuần | [17](17-clean-architecture.md) |
| 8 | Testing (Unit + Integration) | 1 tuần | *(chưa có trong tài liệu)* |
| 9 | Docker, CI/CD, Deploy | 1 tuần | [18](18-docker.md) |

> **Mẹo học:** Đừng chỉ đọc — hãy code theo. Tạo một project thực tế (quản lý sản phẩm, blog, todo app) và áp dụng từng kiến thức vào. Đó là cách học nhanh nhất.
