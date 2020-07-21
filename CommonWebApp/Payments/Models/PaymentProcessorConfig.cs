using System;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Contains general settings for payment processing.
    /// </summary>
    public class PaymentProcessorConfig
    {
        public int ProductIdCacheHours { get; set; } = 72;
        public int OntraportInvoiceId { get; set; } = 0;
        public int PaymentPlanCampaignId { get; set; } = 1;
        public int PayHoursAhead { get; set; } = 20;
        public int DefaultTrialDays { get; set; } = 7;
        public int PaymentFaultDisableAfterDays { get; set; } = 5;
    }
}
