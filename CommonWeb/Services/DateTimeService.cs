using System;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Provides access to the system date.
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        /// <summary>
        /// Get a DateTime object that is set to the current date and time on this computer, expressed as the local time.
        /// </summary>
        public DateTime Now => DateTime.Now;
        /// <summary>
        /// Get a DateTime object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;
        /// <summary>
        /// Get a DateTimeOffset object that is set to the current date and time on this computer, expressed as the local time.
        /// </summary>
        public DateTimeOffset NowOffset => DateTimeOffset.Now;
        /// <summary>
        /// Get a DateTimeOffset object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
    }
}
