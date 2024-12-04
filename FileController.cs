using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IFileStorage _fileStorage;

    // You can inject the appropriate storage class here (e.g., S3Storage, LocalFileSystemStorage, etc.)
    public FileController(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        string fileId = Guid.NewGuid().ToString();  // Generate a unique file ID
        await _fileStorage.UploadFileAsync(fileId, file);
        return Ok(new { fileId });
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadFile(string id)
    {
        return await _fileStorage.DownloadFileAsync(id);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteFile(string id)
    {
        return await _fileStorage.DeleteFileAsync(id);
    }
}
