using System;
using HanumanInstitute.OntraportApi;

namespace HanumanInstitute.CommonWeb.Ontraport
{
    /// <summary>
    /// Provides Ontraport API support for Readings custom objects.
    /// </summary>
    public class OntraportReadings : OntraportBaseCustomObject<ApiReading>, IOntraportReadings
    {
        public OntraportReadings(OntraportHttpClient apiRequest, IOntraportObjects ontraObjects) :
            base(apiRequest, ontraObjects, "Reading", "Readings", ObjectTypeId, null)
        { }

        public const int ObjectTypeId = 10001;
    }
}
