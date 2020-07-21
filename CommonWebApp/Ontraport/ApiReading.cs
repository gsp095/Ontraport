using System;
using HanumanInstitute.OntraportApi.Converters;
using HanumanInstitute.OntraportApi.Models;

namespace HanumanInstitute.CommonWeb.Ontraport
{
    /// <summary>
    /// Reading objects represent a Soul Alignment Reading purchased by the client.
    /// </summary>
    public class ApiReading : ApiCustomObjectBase
    {
        /// <summary>
        /// Returns a ApiProperty object to get or set the ID of the contact who owns this reading.
        /// </summary>
        public ApiProperty<int> ContactIdField => _contactIdField ??= new ApiProperty<int>(this, ContactIdKey);
        private ApiProperty<int>? _contactIdField;
        public const string ContactIdKey = "f1761";
        /// <summary>
        /// Gets or sets the ID of the contact who owns this reading.
        /// </summary>
        public int? ContactId { get => ContactIdField.Value; set => ContactIdField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the name of the client's picture file within the pictures folder.
        /// </summary>
        public ApiPropertyString PictureFileNameField => _pictureFileNameField ??= new ApiPropertyString(this, PictureFileNameKey);
        private ApiPropertyString? _pictureFileNameField;
        public const string PictureFileNameKey = "f1758";
        /// <summary>
        /// Gets or sets the name of the name of the client's picture file within the pictures folder.
        /// </summary>
        public string? PictureFileName { get => PictureFileNameField.Value; set => PictureFileNameField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the client's soul alignment reading.
        /// </summary>
        public ApiPropertyString EnergyReadingField => _energyReadingField ??= new ApiPropertyString(this, EnergyReadingKey);
        private ApiPropertyString? _energyReadingField;
        public const string EnergyReadingKey = "f1759";
        /// <summary>
        /// Gets or sets the client's soul alignment reading.
        /// </summary>
        public string? EnergyReading { get => EnergyReadingField.Value; set => EnergyReadingField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the distance healing report.
        /// </summary>
        public ApiPropertyString DistanceHealingField => _distanceHealingField ??= new ApiPropertyString(this, DistanceHealingKey);
        private ApiPropertyString? _distanceHealingField;
        public const string DistanceHealingKey = "f1760";
        /// <summary>
        /// Gets or sets the distance healing report.
        /// </summary>
        public string? DistanceHealing { get => DistanceHealingField.Value; set => DistanceHealingField.Value = value; }
    }
}
