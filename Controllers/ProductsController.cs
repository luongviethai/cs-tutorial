using Microsoft.AspNetCore.Mvc;

namespace CS_Tutorial.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // Dữ liệu tạm trong bộ nhớ (dùng static để chia sẻ giữa các request)
    private static readonly List<Product> _products =
    [
        new() { Id = 1, Name = "Laptop", Price = 25_000_000 },
        new() { Id = 2, Name = "Chuột không dây", Price = 350_000 },
    ];
    private static int _nextId = 3;

    // GET api/products
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll() => Ok(_products);

    // GET api/products/{id}
    [HttpGet("{id:int}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _products.Find(p => p.Id == id);
        return product is null ? NotFound($"Không tìm thấy sản phẩm có Id = {id}.") : Ok(product);
    }

    // POST api/products
    [HttpPost]
    public ActionResult<Product> Create(Product newProduct)
    {
        newProduct.Id = _nextId++;
        _products.Add(newProduct);
        return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
    }

    // PUT api/products/{id}
    [HttpPut("{id:int}")]
    public ActionResult<Product> Update(int id, Product updatedProduct)
    {
        var product = _products.Find(p => p.Id == id);
        if (product is null)
            return NotFound($"Không tìm thấy sản phẩm có Id = {id}.");

        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;
        return Ok(product);
    }

    // DELETE api/products/{id}
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var product = _products.Find(p => p.Id == id);
        if (product is null)
            return NotFound($"Không tìm thấy sản phẩm có Id = {id}.");

        _products.Remove(product);
        return NoContent();
    }

    // GET api/products/search?name=...
    [HttpGet("search")]
    public ActionResult<IEnumerable<Product>> Search([FromQuery] string? name)
    {
        var result = string.IsNullOrWhiteSpace(name)
            ? _products
            : _products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        return Ok(result);
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}
