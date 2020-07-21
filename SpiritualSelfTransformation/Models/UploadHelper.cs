using System;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace HanumanInstitute.SpiritualSelfTransformation.Models
{
    /// <summary>
    /// Facilitates file management of uploads on the server.
    /// </summary>
    public class UploadHelper : IUploadHelper
    {
        private readonly IFormFileHelper _formFileHelper;
        private readonly IOptions<AppPathsConfig> _appPaths;

        public UploadHelper(IFormFileHelper uploadHelper, IOptions<AppPathsConfig> appPaths)
        {
            _formFileHelper = uploadHelper;
            _appPaths = appPaths;
        }

        /// <summary>
        /// Uploads a picture to the server and returns the file name where it is stored.
        /// </summary>
        /// <param name="formFile">The FormFile containing the picture.</param>
        /// <param name="destFileNameWithoutExt">The destination file name without extension.</param>
        /// <param name="modelState">The view ModelState containing errors.</param>
        /// <param name="fieldName">The display name of field to display in errors.</param>
        /// <returns>The file name where the image was saved, or null.</returns>
        public async Task<string?> UploadClientPicture(IFormFile formFile, string destFileNameWithoutExt, ModelStateDictionary? modelState = null, string fieldName = "")
        {
            var options = _appPaths.Value;
            Func<Image<Rgba32>, Task>? transform = null;
            if (options.UploadPictureMaxWidth > 0 && options.UploadPictureMaxHeight > 0)
            {
                transform = async (img) => await Task.Run(() =>
                    img.ResizeToArea(options.UploadPictureMaxWidth, options.UploadPictureMaxHeight)).ConfigureAwait(false);
            }
            var result = await _formFileHelper.UploadImageAsync(formFile, options.UploadPicturesPath, destFileNameWithoutExt, null, modelState, fieldName, transform).ConfigureAwait(false);
            return result;
        }
    }
}
