using System;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Provides access to the system date.
    /// </summary>
    public interface IDateTimeService
    {
        /// <summary>
        /// Get a DateTime object that is set to the current date and time on this computer, expressed as the local time.
        /// </summary>
        DateTime Now { get; }
        /// <summary>
        /// Get a DateTime object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        DateTime UtcNow { get; }
        /// <summary>
        /// Get a DateTimeOffset object that is set to the current date and time on this computer, expressed as the local time.
        /// </summary>
        DateTimeOffset NowOffset { get; }
        /// <summary>
        /// Get a DateTimeObject object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        DateTimeOffset UtcNowOffset { get; }
    }
}
