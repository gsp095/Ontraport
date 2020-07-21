using System;

namespace HanumanInstitute.CommonWebApp.Ontraport
{
    /// <summary>
    /// Represents a payment plan period unit. This data is stored in Ontraport as dropdown.
    /// </summary>
    public enum PaymentPeriodUnit
    {
        /// <summary>
        /// Billed daily.
        /// </summary>
        Day = 73,
        /// <summary>
        /// Billed every week.
        /// </summary>
        Week = 72,
        /// <summary>
        /// Billed every month.
        /// </summary>
        Month = 71,
        /// <summary>
        /// Billed every year.
        /// </summary>
        Year = 70,
        /// <summary>
        /// Postpaid subscription billed on the first of each month.
        /// </summary>
        MonthPostpaid = 69,
        /// <summary>
        /// Postpaid subscription on trial period.
        /// </summary>
        MonthPostpaidTrial = 68
    }
}
