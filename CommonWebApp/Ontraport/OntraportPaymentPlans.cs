using System;
using HanumanInstitute.OntraportApi;

namespace HanumanInstitute.CommonWeb.Ontraport
{
    /// <summary>
    /// Provides Ontraport API support for PaymentPlans custom objects.
    /// </summary>
    public class OntraportPaymentPlans : OntraportBaseCustomObject<ApiPaymentPlan>, IOntraportPaymentPlans
    {
        public OntraportPaymentPlans(OntraportHttpClient apiRequest, IOntraportObjects ontraObjects) :
            base(apiRequest, ontraObjects, "PaymentPlan", "PaymentPlans", ObjectTypeId, null)
        { }

        public const int ObjectTypeId = 10002;
    }
}
