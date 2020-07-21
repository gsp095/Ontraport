using System;
using System.ComponentModel.DataAnnotations;

namespace HanumanInstitute.SpiritualSelfTransformation.Models
{
    public class AppPathsConfig
    {
        [Required]
        public string AdminPassword { get; set; } = string.Empty;
        [Required]
        public string UploadPicturesPath { get; set; } = string.Empty;
        [Required]
        public Uri? UploadPicturesUrl { get; set; }
        [Required]
        public string UploadRecordingsPath { get; set; } = string.Empty;
        [Required]
        public Uri? UploadRecordingsUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int UploadPictureMaxWidth { get; set; } = 640;
        [Range(0, int.MaxValue)]
        public int UploadPictureMaxHeight { get; set; } = 480;
    }
}
