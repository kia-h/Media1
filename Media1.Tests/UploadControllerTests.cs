using Media1.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Media1.Tests
{
    public class UploadControllerTests
    {
        private readonly string _tempDir;

        public UploadControllerTests()
        {
            // temp folder for uploads
            _tempDir = Path.Combine(Path.GetTempPath(), "Media1Tests_" + Guid.NewGuid());
            Directory.CreateDirectory(_tempDir);
        }

        private UploadController CreateController(FileUploadSettings? customSettings = null)
        {
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.ContentRootPath).Returns(_tempDir);

            var settings = customSettings ?? new FileUploadSettings
            {
                MaxFileSizeMB = 200,
                AllowedExtensions = new[] { ".mp4", ".mov", ".avi" },
                SupportedExtensions = new[] { ".mp4" },
                UploadPath = "media"
            };

            return new UploadController(envMock.Object, settings);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenNoFilesProvided()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.Upload(new List<IFormFile>());

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No files selected.", badRequest.Value);
        }

        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenExtensionNotAllowed()
        {
            // Arrange
            var controller = CreateController();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.Length).Returns(100);

            // Act
            var result = await controller.Upload(new List<IFormFile> { fileMock.Object });

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains(".mp4", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Upload_ReturnsOk_WhenValidMp4FileProvided()
        {
            // Arrange
            var controller = CreateController();

            var fileMock = new Mock<IFormFile>();
            var fileName = "test.mp4";
            var filePath = Path.Combine(_tempDir, fileName);
            var fileContent = new MemoryStream(new byte[100]); // dummy data

            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(fileContent.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(fileContent);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Callback<Stream, System.Threading.CancellationToken>((s, _) =>
                {
                    fileContent.Position = 0;
                    fileContent.CopyTo(s);
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.Upload(new List<IFormFile> { fileMock.Object });

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.True(File.Exists(Path.Combine(_tempDir, "media", fileName)));
        }

        [Fact]
        public void List_ReturnsOnlySupportedExtensions()
        {
            // Arrange
            var controller = CreateController();

            var uploadDir = Path.Combine(_tempDir, "media");
            Directory.CreateDirectory(uploadDir);
            File.WriteAllText(Path.Combine(uploadDir, "video.mp4"), "test");

            // Act
            var result = controller.List() as OkObjectResult;
            var files = Assert.IsAssignableFrom<IEnumerable<dynamic>>(result.Value);
            var first = files.First();
            var fileNameProp = first.GetType().GetProperty("FileName")?.GetValue(first)?.ToString();

            // Assert
            Assert.Single(files); // only .mp4 should be listed
            Assert.Contains("video.mp4", fileNameProp);
        }
    }
}
