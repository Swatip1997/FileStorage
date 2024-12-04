using Microsoft.AspNetCore.Mvc;

namespace FileStorageApi.Tests
{
    public class FileControllerTests
    {
        private readonly Mock<IFileStorage> _mockFileStorage;
        private readonly FileController _controller;

        public FileControllerTests()
        {
            // Set up the mock IFileStorage
            _mockFileStorage = new Mock<IFileStorage>();

            // Inject the mock IFileStorage into the controller
            _controller = new FileController(_mockFileStorage.Object);
        }

        [Fact]
        public async Task UploadFile_ShouldReturnOkResult_WhenFileIsUploaded()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(10);
            file.Setup(f => f.OpenReadStream()).Returns(new System.IO.MemoryStream(new byte[10]));

            _mockFileStorage.Setup(fs => fs.UploadFileAsync(It.IsAny<string>(), file.Object))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UploadFile(file.Object);

            object Assert = null;
            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var value = actionResult.Value as dynamic;
            Assert.NotNull(value.fileId);
        }

        [Fact]
        public async Task DownloadFile_ShouldReturnFileContent_WhenFileExists()
        {
            // Arrange
            var fileId = "123";
            var mockFileResult = new FileContentResult(new byte[] { 1, 2, 3 }, "application/octet-stream");

            _mockFileStorage.Setup(fs => fs.DownloadFileAsync(fileId))
                .ReturnsAsync(mockFileResult);

            // Act
            var result = await _controller.DownloadFile(fileId);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/octet-stream", fileResult.ContentType);
            Assert.Equal(new byte[] { 1, 2, 3 }, fileResult.FileContents);
        }

        [Fact]
        public async Task DownloadFile_ShouldReturnNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            var fileId = "invalid-id";

            _mockFileStorage.Setup(fs => fs.DownloadFileAsync(fileId))
                .ReturnsAsync(new NotFoundResult());

            // Act
            var result = await _controller.DownloadFile(fileId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteFile_ShouldReturnOkResult_WhenFileIsDeleted()
        {
            // Arrange
            var fileId = "123";

            _mockFileStorage.Setup(fs => fs.DeleteFileAsync(fileId))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _controller.DeleteFile(fileId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteFile_ShouldReturnNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            var fileId = "invalid-id";

            _mockFileStorage.Setup(fs => fs.DeleteFileAsync(fileId))
                .ReturnsAsync(new NotFoundResult());

            // Act
            var result = await _controller.DeleteFile(fileId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }

    internal class Mock<T>
    {
    }
}
