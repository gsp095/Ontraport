using System;
using HanumanInstitute.OntraportApi.Converters;
using HanumanInstitute.OntraportApi.Models;

namespace HanumanInstitute.CommonWeb.Ontraport
{
    /// <summary>
    /// Recording objects represent a coaching call recording file that is sent via email.
    /// </summary>
    public class ApiRecording : ApiCustomObjectBase
    {
        /// <summary>
        /// Returns a ApiProperty object to get or set the ID of the contact who owns this recording.
        /// </summary>
        public ApiProperty<int> ContactIdField => _contactIdField ??= new ApiProperty<int>(this, ContactIdKey);
        private ApiProperty<int>? _contactIdField;
        public const string ContactIdKey = "f1745";
        /// <summary>
        /// Gets or sets the ID of the contact who owns this recording.
        /// </summary>
        public int? ContactId { get => ContactIdField.Value; set => ContactIdField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the name of the recording file within the client recordings folder.
        /// </summary>
        public ApiPropertyString FileNameField => _fileNameField ??= new ApiPropertyString(this, FileNameKey);
        private ApiPropertyString? _fileNameField;
        public const string FileNameKey = "f1746";
        /// <summary>
        /// Gets or sets the name of the recording file within the client recordings folder.
        /// </summary>
        public string? FileName { get => FileNameField.Value; set => FileNameField.Value = value; }
    }
}
