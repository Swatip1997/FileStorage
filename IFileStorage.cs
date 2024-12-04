using Microsoft.AspNetCore.Mvc;

public interface IFileStorage
{
    Task UploadFileAsync(string identifier, IFormFile file);
    Task<IActionResult> DownloadFileAsync(string identifier);
    Task<IActionResult> DeleteFileAsync(string identifier);
}
