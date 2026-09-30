# 15. File Upload

```csharp
[HttpPost("upload")]
[RequestSizeLimit(10 * 1024 * 1024)]  // Giới hạn 10MB
public async Task<ActionResult<string>> Upload([FromForm] IFormFile file)
{
    if (file.Length == 0)
        return BadRequest("File trống");

    // Kiểm tra loại file
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (!allowedExtensions.Contains(extension))
        return BadRequest("Loại file không được hỗ trợ");

    // Tạo tên file unique
    var fileName = $"{Guid.NewGuid()}{extension}";
    var filePath = Path.Combine("wwwroot", "uploads", fileName);

    // Lưu file
    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
    await using var stream = new FileStream(filePath, FileMode.Create);
    await file.CopyToAsync(stream);

    var url = $"/uploads/{fileName}";
    return Ok(new { url });
}

// Upload nhiều file
[HttpPost("upload-multiple")]
public async Task<ActionResult> UploadMultiple([FromForm] List<IFormFile> files)
{
    var urls = new List<string>();
    foreach (var file in files)
    {
        // ... xử lý từng file
    }
    return Ok(new { urls });
}
```

---

[← 14. Logging](14-logging.md) · [Mục lục](README.md) · [16. Background Services →](16-background-services.md)
