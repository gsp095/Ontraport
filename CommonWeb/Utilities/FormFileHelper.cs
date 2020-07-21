using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using Res = HanumanInstitute.CommonWeb.Properties.Resources;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Validates, parses and saves uploaded Http files.
    /// </summary>
    public class FormFileHelper : IFormFileHelper
    {
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IFileSystemService _fileSystem;

        public FormFileHelper(IWebHostEnvironment hostingEnv, IFileSystemService fileSystem)
        {
            _hostEnv = hostingEnv;
            _fileSystem = fileSystem;
        }

        public IEnumerable<string> ValidExtensions { get; set; } = new string[] { ".JPG", ".GIF", ".PNG" };

        private static bool CheckFormFileNotNull([NotNullWhen(true)] IFormFile? formFile, ModelStateDictionary? modelState, string fieldName)
        {
            if (formFile == null)
            {
                modelState?.AddModelError("", string.Format(CultureInfo.InvariantCulture, "{0} is required.", fieldName));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Uploads a file stream to the server.
        /// </summary>
        /// <param name="formFile">The FormFile containing the file.</param>
        /// <param name="modelState">The view ModelState containing errors.</param>
        /// <param name="uploadDirectory">The directory in which to store the file.</param>
        /// <param name="destFileName">The destination file name.</param>
        /// <param name="modelState">The view ModelState containing errors.</param>
        /// <param name="fieldName">The display name of field to display in errors.</param>
        public async Task UploadFileAsync(IFormFile? formFile, string uploadDirectory, string destFileName, ModelStateDictionary? modelState = null, string fieldName = "")
        {
            fieldName = fieldName.Default("File");
            destFileName.CheckNotNullOrEmpty(nameof(destFileName));

            if (!CheckFormFileNotNull(formFile, modelState, fieldName) || formFile == null) { return; } // suppress warning below

            // Save image in upload folder.
            var filePath = GetFilePathAbsolute(uploadDirectory, destFileName);
            using var outStream = _fileSystem.CreateFileStream(filePath);
            using var inStream = formFile.OpenReadStream();
            await inStream.CopyToAsync(outStream).ConfigureAwait(false);
        }

        /// <summary>
        /// Uploads an image file to the server.
        /// </summary>
        /// <param name="formFile">The FormFile containing the picture.</param>
        /// <param name="uploadDirectory">The directory in which to store the file.</param>
        /// <param name="destFileNameWithoutExt">The destination file name without extension.</param>
        /// <param name="destFileFormat">The image file format extension to save, or null to keep the original image format.</param>
        /// <param name="modelState">The view ModelState containing errors.</param>
        /// <param name="fieldName">The display name of field to display in errors.</param>
        /// <param name="transform">A function to transform the image before saving.</param>
        /// <returns>The file name where the image was saved, or null.</returns>
        public async Task<string?> UploadImageAsync(IFormFile? formFile, string uploadDirectory, string destFileNameWithoutExt, string? destFileFormat = null,
            ModelStateDictionary? modelState = null, string fieldName = "", Func<Image<Rgba32>, Task>? transform = null)
        {
            fieldName = fieldName.Default("File");
            if (!CheckFormFileNotNull(formFile, modelState, fieldName) || formFile == null) { return null; } // suppress warning below

            var upload = await ProcessAsImageAsync(formFile, modelState).ConfigureAwait(false);
            if (upload != null)
            {
                destFileFormat = string.IsNullOrEmpty(destFileFormat) ? Path.GetExtension(formFile.FileName) : destFileFormat;
                return await SaveImageAsync(upload, uploadDirectory, destFileNameWithoutExt, destFileFormat, transform).ConfigureAwait(false);
            }
            return null;
        }

        private string GetFilePathAbsolute(string uploadDirectory, string fileName)
        {
            var result = Path.Combine(_hostEnv.ContentRootPath ?? "", uploadDirectory ?? "", fileName);
            _fileSystem.EnsureDirectoryExists(result);
            return result;
        }

        /// <summary>
        /// Validates and parses a FormFile into an Image, adding any errors into modelState.
        /// </summary>
        /// <param name="formFile">The uploaded Http file.</param>
        /// <param name="modelState">The view ModelState containing errors.</param>
        /// <param name="fieldName">The display name of field to display in errors.</param>
        /// <param name="maxFileSizeMb">The maximum file size in MB, or null.</param>
        /// <returns>The image if FormFile is valid, otherwise null.</returns>
        public async Task<Image<Rgba32>?> ProcessAsImageAsync(IFormFile? formFile, ModelStateDictionary? modelState = null, string fieldName = "Image", int? maxFileSizeMb = null)
        {
            modelState.CheckNotNull(nameof(modelState));
            if (maxFileSizeMb != null && maxFileSizeMb <= 0)
            {
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, Res.ValueMustBeNullOrPositive, nameof(maxFileSizeMb)));
            }

            if (formFile == null)
            {
                modelState?.AddModelError("",
                    string.Format(CultureInfo.InvariantCulture, Res.FieldRequired, fieldName));
                return null;
            }

            var fileName = Path.GetFileName(formFile.FileName);

            if (formFile.Length == 0)
            {
                modelState?.AddModelError(formFile.Name,
                    string.Format(CultureInfo.InvariantCulture, Res.FileIsEmpty, fieldName, fileName));
            }
            else if (formFile.Length > maxFileSizeMb * 1024 * 1024)
            {
                modelState?.AddModelError(formFile.Name,
                    string.Format(CultureInfo.InvariantCulture, Res.FileExceedsMaxSize, fieldName, fileName, maxFileSizeMb));
            }
            else
            {
                // Check file extension (must be JPG, GIF or PNG).
                if (fileName == null || !ValidExtensions.Any(x => fileName.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    modelState?.AddModelError(formFile.Name,
                        string.Format(CultureInfo.InvariantCulture, Res.FileIsNotValidImage, fieldName, fileName));
                }
                else
                {
                    // Ensure file is valid.
                    try
                    {
                        var result = await Task.Run(() => Image.Load(formFile.OpenReadStream())).ConfigureAwait(false);
                        return result;
                    }
                    catch (NotSupportedException)
                    {
                        modelState?.AddModelError(formFile.Name,
                           string.Format(CultureInfo.InvariantCulture, Res.FileIsNotValidImage, fieldName, fileName));
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Saves an image as a file in the server's upload folder.
        /// </summary>
        /// <param name="image">The image to save as a file.</param>
        /// <param name="uploadDirectory">The directory in which to store the file.</param>
        /// <param name="destFileNameWithoutExt">The destination file name without extension.</param>
        /// <param name="destFileFormat">The image file format extension to save.</param>
        /// <param name="transform">A function to transform the image before saving.</param>
        /// <returns>The file name where the image was saved, or null.</returns>
        public async Task<string> SaveImageAsync(Image<Rgba32> image, string uploadDirectory, string destFileNameWithoutExt, string destFileFormat, Func<Image<Rgba32>, Task>? transform = null)
        {
            image.CheckNotNull(nameof(image));
            destFileNameWithoutExt.CheckNotNullOrEmpty(nameof(destFileNameWithoutExt));
            destFileFormat.CheckNotNullOrEmpty(nameof(destFileFormat));

            if (transform != null)
            {
                await transform(image).ConfigureAwait(false);
            }

            // Save image in upload folder.
            destFileFormat = destFileFormat.StartsWith('.') ? destFileFormat : "." + destFileFormat;
            var fileName = destFileNameWithoutExt + destFileFormat;
            var filePath = GetFilePathAbsolute(uploadDirectory, fileName);
            await Task.Run(() => _fileSystem.SaveImage<Rgba32>(image, filePath)).ConfigureAwait(false);

            return fileName;
        }
    }
}
