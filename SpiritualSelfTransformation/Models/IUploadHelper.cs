using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HanumanInstitute.SpiritualSelfTransformation.Models
{
    /// <summary>
    /// Facilitates file management of uploads on the server.
    /// </summary>
    public interface IUploadHelper
    {
        /// <summary>
        /// Uploads a picture to the server and returns the file name where it is stored.
        /// </summary>
        /// <param name="formFile">The FormFile containing the picture.</param>
        /// <param name="destFileNameWithoutExt">The destination file name without extension.</param>
        /// <param name="modelState">The view ModelState containing errors.</param>
        /// <param name="fieldName">The display name of field to display in errors.</param>
        /// <returns>The file name where the image was saved, or null.</returns>
        Task<string?> UploadClientPicture(IFormFile formFile, string destFileNameWithoutExt, ModelStateDictionary? modelState = null, string fieldName = "");
    }
}
