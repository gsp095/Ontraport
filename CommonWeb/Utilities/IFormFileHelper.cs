using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Validates, parses and saves uploaded Http files.
    /// </summary>
    public interface IFormFileHelper
    {
        /// <summary>
        /// Uploads a file stream to the server.
        /// </summary>
        /// <param name="formFile">The FormFile containing the file.</param>
        /// <param name="modelState">The view ModelState containing errors.</param>
        /// <param name="uploadDirectory">The directory in which to store the file.</param>
        /// <param name="destFileName">The destination file name.</param>
        /// <param name="modelState">The view ModelState containing errors.</param>
        /// <param name="fieldName">The display name of field to display in errors.</param>
        Task UploadFileAsync(IFormFile? formFile, string uploadDirectory, string destFileName, ModelStateDictionary? modelState = null, string fieldName = "");
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
        Task<string?> UploadImageAsync(IFormFile? formFile, string uploadDirectory, string destFileNameWithoutExt, string? destFileFormat = null, ModelStateDictionary? modelState = null, string fieldName = "File", Func<Image<Rgba32>, Task>? transform = null);
        /// <summary>
        /// Validates and parses a FormFile into an Image, adding any errors into modelState.
        /// </summary>
        /// <param name="formFile">The uploaded Http file.</param>
        /// <param name="modelState">The ModelState containing page errors.</param>
        /// <param name="fieldName">The display name of field to display in errors.</param>
        /// <param name="maxFileSizeMb">The maximum file size in MB, or null.</param>
        /// <returns>The image if FormFile is valid, otherwise null.</returns>
        Task<Image<Rgba32>?> ProcessAsImageAsync(IFormFile? formFile, ModelStateDictionary modelState, string fieldName = "Image", int? maxFileSizeMb = null);
        /// <summary>
        /// Saves an image as a file in the server's upload folder.
        /// </summary>
        /// <param name="image">The image to save as a file.</param>
        /// <param name="uploadDirectory">The directory in which to store the file.</param>
        /// <param name="destFileNameWithoutExt">The destination file name without extension.</param>
        /// <param name="destFileFormat">The image file format extension to save.</param>
        /// <param name="transform">A function to transform the image before saving.</param>
        /// <returns>The file name where the image was saved, or null.</returns>
        Task<string> SaveImageAsync(Image<Rgba32> image, string uploadDirectory, string destFileNameWithoutExt, string destFileFormat, Func<Image<Rgba32>, Task>? transform = null);
    }
}
