# 4. Controller — Xử Lý Request

## 4.1 Controller cơ bản (CRUD hoàn chỉnh)

```csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]                    // Bật tính năng API (auto validation, auto 400...)
[Route("api/[controller]")]        // Route: api/products
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    // Dependency Injection qua constructor
    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    // ===== GET api/products =====
    // Lấy danh sách sản phẩm (có phân trang, tìm kiếm)
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        _logger.LogInformation("Lấy danh sách sản phẩm, trang {Page}", page);
        var result = await _productService.GetAllAsync(page, pageSize, search);
        return Ok(result);
    }

    // ===== GET api/products/5 =====
    // Lấy chi tiết một sản phẩm
    [HttpGet("{id:int}")]           // {id:int} = route constraint, chỉ chấp nhận số
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
            return NotFound(new { message = $"Không tìm thấy sản phẩm với id = {id}" });

        return Ok(product);
    }

    // ===== POST api/products =====
    // Tạo sản phẩm mới
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request)
    {
        // [ApiController] tự động validate và trả 400 nếu request không hợp lệ
        var product = await _productService.CreateAsync(request);

        // Trả về 201 Created + header Location: api/products/{id}
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // ===== PUT api/products/5 =====
    // Cập nhật toàn bộ sản phẩm
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateProductRequest request)
    {
        var exists = await _productService.ExistsAsync(id);
        if (!exists)
            return NotFound();

        await _productService.UpdateAsync(id, request);
        return NoContent(); // 204 — cập nhật thành công, không có body
    }

    // ===== DELETE api/products/5 =====
    // Xóa sản phẩm
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var exists = await _productService.ExistsAsync(id);
        if (!exists)
            return NotFound();

        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
```

## 4.2 Các kiểu trả về thường dùng

```csharp
return Ok(data);                  // 200 — Thành công, có dữ liệu
return NoContent();               // 204 — Thành công, không có dữ liệu
return Created(uri, data);        // 201 — Tạo mới thành công
return CreatedAtAction(...);      // 201 — Tạo mới + link đến resource
return BadRequest(errors);        // 400 — Request không hợp lệ
return Unauthorized();            // 401 — Chưa đăng nhập
return Forbid();                  // 403 — Không có quyền
return NotFound();                // 404 — Không tìm thấy
return Conflict();                // 409 — Xung đột (ví dụ email đã tồn tại)
return StatusCode(500, "Lỗi");    // 500 — Lỗi server (ít dùng trực tiếp)
```

## 4.3 Binding dữ liệu từ Request

```csharp
// [FromBody]  — Dữ liệu từ JSON body (POST, PUT)
[HttpPost]
public ActionResult Create([FromBody] CreateProductRequest request) { }

// [FromQuery] — Dữ liệu từ query string (?page=1&search=abc)
[HttpGet]
public ActionResult Search([FromQuery] string search, [FromQuery] int page = 1) { }

// [FromRoute] — Dữ liệu từ URL path (/api/products/5)
[HttpGet("{id}")]
public ActionResult GetById([FromRoute] int id) { }

// [FromHeader] — Dữ liệu từ HTTP header
[HttpGet]
public ActionResult Get([FromHeader(Name = "X-Api-Key")] string apiKey) { }

// [FromForm] — Dữ liệu từ form (file upload)
[HttpPost("upload")]
public ActionResult Upload([FromForm] IFormFile file) { }
```

---

[← 3. Program.cs](03-program-cs.md) · [Mục lục](README.md) · [5. Minimal APIs →](05-minimal-apis.md)
