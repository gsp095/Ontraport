using System;
using HanumanInstitute.OntraportApi;

namespace HanumanInstitute.CommonWeb.Ontraport
{
    /// <summary>
    /// Provides Ontraport API support for Recording custom objects.
    /// </summary>
    public class OntraportRecordings : OntraportBaseCustomObject<ApiRecording>, IOntraportRecordings
    {
        public OntraportRecordings(OntraportHttpClient apiRequest, IOntraportObjects ontraObjects) :
            base(apiRequest, ontraObjects, "Recording", "Recordings", ObjectTypeId, ApiRecording.FileNameKey)
        { }

        public const int ObjectTypeId = 10000;
    }
}
