using System;
using System.IO;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;

namespace HanumanInstitute.CommonWeb.Tests
{
    public class FormFileHelperTests
    {
        private const string DefaultContentRootPath = "C:\\uploads";

        public ModelStateDictionary ModelState => _modelState ??= new ModelStateDictionary();
        private ModelStateDictionary? _modelState;

        public IWebHostEnvironment FakeHostEnv => _fakeHostEnv ??= Mock.Of<IWebHostEnvironment>(x => x.ContentRootPath == DefaultContentRootPath);
        private IWebHostEnvironment? _fakeHostEnv;

        public Mock<IFileSystemService> FakeFileSystem => _fakeFileSystem ??= new Mock<IFileSystemService>();
        private Mock<IFileSystemService>? _fakeFileSystem;

        public IFormFileHelper Model => _model ??= new FormFileHelper(FakeHostEnv, FakeFileSystem.Object);
        private IFormFileHelper? _model;

        private static IFormFile GetFormFile(string fileName = "file.jpg", string name = "", int length = 1)
        {
            var fakeFile = Mock.Of<IFormFile>(x =>
                x.Length == length &&
                x.Name == name &&
                x.FileName == fileName &&
                x.OpenReadStream() == GetImageStream());
            return fakeFile;
        }

        private static Stream GetImageStream()
        {
            var result = new MemoryStream();
            using var image = new Image<Rgba32>(1, 1);
            image.SaveAsJpeg(result);
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }

        [Theory]
        [InlineData("/uploads/", "file1", @"/uploads/file1")]
        [InlineData("uploads", "file1.jpg", @"uploads\file1.jpg")]
        [InlineData("~/uploads/", "file1.jpg", @"/uploads/file1.jpg")]
        [InlineData(@"~\uploads\", "file1.jpg", @"\uploads\file1.jpg")]
        [InlineData("", "file1.jpg", @"file1.jpg")]
        [InlineData(null, "file1.jpg", @"file1.jpg")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed: It gets disposed by the calling class.")]
        public async Task UploadFileAsync_ValidPath_CreateFileAtExpectedPath(string uploadDirectory, string fileName, string expectedPath)
        {
            Mock.Get(FakeHostEnv).Setup(x => x.ContentRootPath).Returns(uploadDirectory);
            var formFile = GetFormFile();
            string? filePath = null;
            FakeFileSystem.Setup(x => x.CreateFileStream(It.IsAny<string>()))
                .Callback<string>((path) => filePath = path)
                .Returns(new MemoryStream());

            await Model.UploadFileAsync(formFile, uploadDirectory, fileName);

            Assert.EndsWith(expectedPath, filePath, StringComparison.InvariantCulture);
        }

        [Fact]
        public async Task UploadImageAsync_NullFormFile_ErrorInModelState()
        {
            var result = await Model.UploadImageAsync(null, "uploads", "filename", ".jpg", ModelState);

            Assert.Null(result);
            Assert.Single(ModelState);
        }

        [Fact]
        public async Task UploadImageAsync_InvalidFormFile_ErrorInModelState()
        {
            var fakeFile = Mock.Of<IFormFile>(x =>
                x.Length == 10 &&
                x.Name == "name" &&
                x.FileName == "filename.jpg" &&
                x.OpenReadStream() == new MemoryStream(10));

            var result = await Model.UploadImageAsync(fakeFile, "uploads", "filename", ".jpg", ModelState);

            Assert.Null(result);
            Assert.Single(ModelState);
        }

        [Theory]
        [InlineData("local.jpg", null, "file")]
        public async Task UploadImageAsync_WithResize_ImageHasNewSize(string sourceFileName, string uploadDirectory, string destFileNameWithoutExt)
        {
            var fakeFile = GetFormFile(sourceFileName);
            int newWidth = 100, newHeight = 100;
            Image<Rgba32>? savedImage = null;
            FakeFileSystem.Setup(x => x.SaveImage<Rgba32>(It.IsAny<Image<Rgba32>>(), It.IsAny<string>()))
                .Callback<Image<Rgba32>, string>((img, path) => savedImage = img);

            var result = await Model.UploadImageAsync(fakeFile, uploadDirectory, destFileNameWithoutExt, null, ModelState, transform:
                async (pic) => await Task.Run(() => pic.Mutate(x => x.Resize(newWidth, newHeight)))
            );

            Assert.NotNull(result);
            Assert.Empty(ModelState);
            Assert.NotNull(savedImage);
            Assert.Equal(newWidth, savedImage?.Width);
            Assert.Equal(newHeight, savedImage?.Height);
        }

        [Theory]
        [InlineData("local.jpg", null, "file", ".png", "C:\\uploads\\file.png", "file.png")]
        [InlineData("local.jpg", "", "file", null, "C:\\uploads\\file.jpg", "file.jpg")]
        [InlineData("local.jpg", "images", "file", null, "C:\\uploads\\images\\file.jpg", "file.jpg")]
        public async Task UploadImageAsync_ValidFormFile_SaveAsExpected(string sourceFileName, string uploadDirectory, string destFileNameWithoutExt, string destFileFormat, string expectedSaveAs, string expectedFile)
        {
            var mockFile = GetFormFile(sourceFileName);
            string? saveAsPath = null;
            FakeFileSystem.Setup(x => x.SaveImage(It.IsAny<Image<Rgba32>>(), It.IsAny<string>()))
                .Callback<Image<Rgba32>, string>((img, saveAs) => saveAsPath = saveAs);

            var result = await Model.UploadImageAsync(mockFile, uploadDirectory, destFileNameWithoutExt, destFileFormat, ModelState);

            Assert.NotNull(result);
            Assert.Empty(ModelState);
            Assert.Equal(expectedSaveAs, saveAsPath);
            Assert.Equal(expectedFile, result);
        }

        [Theory]
        [InlineData("C:\\", "", "file", ".png", "C:\\file.png", "file.png")]
        [InlineData("C:\\", "", "file", "png", "C:\\file.png", "file.png")]
        [InlineData("C:\\", null, "file", "png", "C:\\file.png", "file.png")]
        [InlineData(null, "uploads", "file", "png", "uploads\\file.png", "file.png")]
        [InlineData("C:\\Files", "uploads\\client-recordings", "a@email.com", ".jpg", "C:\\Files\\uploads\\client-recordings\\a@email.com.jpg", "a@email.com.jpg")]
        [InlineData("C:\\Files", "client-recordings\\", "a@email.com", ".jpg", "C:\\Files\\client-recordings\\a@email.com.jpg", "a@email.com.jpg")]
        public async Task SaveImageAsync_SaveAsInfo_SaveAsExpected(string contentRootPath, string uploadDirectory, string destFileNameWithoutExt, string destFileFormat, string expectedSaveAs, string expectedFile)
        {
            Mock.Get(FakeHostEnv).Setup(x => x.ContentRootPath).Returns(contentRootPath);
            using var image = new Image<Rgba32>(1, 1);
            string? saveAsPath = null;
            FakeFileSystem.Setup(x => x.SaveImage(It.IsAny<Image<Rgba32>>(), It.IsAny<string>()))
                .Callback<Image<Rgba32>, string>((img, saveAs) => saveAsPath = saveAs);

            var result = await Model.SaveImageAsync(image, uploadDirectory, destFileNameWithoutExt, destFileFormat, null);

            Assert.Equal(expectedSaveAs, saveAsPath);
            Assert.Equal(expectedFile, result);
            Assert.Empty(ModelState);
        }

        [Fact]
        public async Task ProcessAsImage_NullFormFile_ErrorInModalState()
        {
            await Model.ProcessAsImageAsync(null, ModelState);

            Assert.Single(ModelState);
        }

        [Fact]
        public async Task ProcessAsImage_EmptyFile_ErrorInModalState()
        {
            var mockFile = Mock.Of<IFormFile>(x =>
                x.Length == 0 &&
                x.Name == " ");

            await Model.ProcessAsImageAsync(mockFile, ModelState);

            Assert.Single(ModelState);
        }

        [Fact]
        public async Task ProcessAsImage_LargeFile_ErrorInModalState()
        {
            var mockFile = Mock.Of<IFormFile>(x =>
                x.Length == int.MaxValue &&
                x.Name == " ");

            await Model.ProcessAsImageAsync(mockFile, ModelState);

            Assert.Single(ModelState);
        }

        [Fact]
        public async Task ProcessAsImage_FileNotImage_ErrorInModalState()
        {
            var mockFile = Mock.Of<IFormFile>(x =>
                x.Length == 1 &&
                x.Name == " " &&
                x.FileName == "file.txt");

            await Model.ProcessAsImageAsync(mockFile, ModelState);

            Assert.Single(ModelState);
        }

        [Fact]
        public async Task ProcessAsImage_ValidImage_ReturnsImage()
        {
            var mockFile = GetFormFile();

            var result = await Model.ProcessAsImageAsync(mockFile, ModelState);

            Assert.NotNull(result);
            Assert.Empty(ModelState);
        }
    }
}
