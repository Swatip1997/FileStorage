using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;

public class S3Storage : IFileStorage
{
    private readonly string _bucketName = "your-bucket-name";
    private readonly IAmazonS3 _s3Client;

    public S3Storage()
    {
        _s3Client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1); // Use your S3 region
    }

    public async Task UploadFileAsync(string identifier, IFormFile file)
    {
        var fileTransferUtility = new TransferUtility(_s3Client);
        using (var stream = file.OpenReadStream())
        {
            await fileTransferUtility.UploadAsync(stream, _bucketName, identifier);
        }
    }

    public async Task<IActionResult> DownloadFileAsync(string identifier)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = identifier
        };

        try
        {
            var response = await _s3Client.GetObjectAsync(request);
            return new FileStreamResult(response.ResponseStream, response.Headers["Content-Type"]);
        }
        catch (Exception ex)
        {
            return new NotFoundResult();
        }
    }

    public async Task<IActionResult> DeleteFileAsync(string identifier)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = identifier
        };

        await _s3Client.DeleteObjectAsync(request);
        return new OkResult();
    }
}
