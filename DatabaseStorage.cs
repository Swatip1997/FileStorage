using Dapper;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;

public class DatabaseStorage : IFileStorage
{
    private readonly string _connectionString = "your-connection-string";

    public async Task UploadFileAsync(string identifier, IFormFile file)
    {
        var filePath = Path.Combine("C:\\Storage", identifier); // Local file storage path (just for metadata)
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            string query = "INSERT INTO FileMetadata (FileId, FileName, FilePath) VALUES (@FileId, @FileName, @FilePath)";
            await connection.ExecuteAsync(query, new { FileId = identifier, FileName = file.FileName, FilePath = filePath });
        }
    }

    public async Task<IActionResult> DownloadFileAsync(string identifier)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT FilePath FROM FileMetadata WHERE FileId = @FileId";
            var filePath = await connection.QueryFirstOrDefaultAsync<string>(query, new { FileId = identifier });

            if (filePath != null && File.Exists(filePath))
            {
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                return new FileContentResult(fileBytes, "application/octet-stream");
            }

            return new NotFoundResult();
        }
    }

    public async Task<IActionResult> DeleteFileAsync(string identifier)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            string query = "DELETE FROM FileMetadata WHERE FileId = @FileId";
            await connection.ExecuteAsync(query, new { FileId = identifier });
        }

        return new OkResult();
    }
}
