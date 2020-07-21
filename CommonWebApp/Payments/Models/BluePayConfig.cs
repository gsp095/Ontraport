using System;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Contains configuration settings for BluePay payment processing.
    /// </summary>
    public class BluePayConfig
    {
        public string AccountIdUsd { get; set; } = string.Empty;
        public string SecretKeyUsd { get; set; } = string.Empty;
        public string AccountIdCad { get; set; } = string.Empty;
        public string SecretKeyCad { get; set; } = string.Empty;
        public bool TestMode { get; set; } = false;
    }
}
