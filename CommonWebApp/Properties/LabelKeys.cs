using System;
using System.ComponentModel;
using System.Globalization;
using HanumanInstitute.CommonWebApp.Properties;

namespace HanumanInstitute.CommonWebApp
{
    /// <summary>
    /// Localized display name attribute
    /// </summary>
    internal sealed class DisplayLocalizedAttribute : DisplayNameAttribute
    {
        readonly string _resourceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedDisplayNameAttribute"/> class.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        public DisplayLocalizedAttribute(string resourceName)
        : base()
        {
            _resourceName = resourceName;
        }

        /// <summary>
        /// Gets the display name for a property, event, or public void method that takes no arguments stored in this attribute.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The display name.
        /// </returns>
        public override string? DisplayName
        {
            get
            {
                return Labels.ResourceManager.GetString(this._resourceName, CultureInfo.CurrentCulture);
            }
        }
    }

    internal static class LabelKeys
    {
        public const string ProcessOrderAddressAddress = "ProcessOrderAddressAddress";
        public const string ProcessOrderAddressAddress2 = "ProcessOrderAddressAddress2";
        public const string ProcessOrderAddressCity = "ProcessOrderAddressCity";
        public const string ProcessOrderAddressCountry = "ProcessOrderAddressCountry";
        public const string ProcessOrderAddressEmail = "ProcessOrderAddressEmail";
        public const string ProcessOrderAddressFirstName = "ProcessOrderAddressFirstName";
        public const string ProcessOrderAddressLastName = "ProcessOrderAddressLastName";
        public const string ProcessOrderAddressPhone = "ProcessOrderAddressPhone";
        public const string ProcessOrderAddressPostalCode = "ProcessOrderAddressPostalCode";
        public const string ProcessOrderAddressReferral = "ProcessOrderAddressReferral";
        public const string ProcessOrderAddressState = "ProcessOrderAddressState";
    }
}
