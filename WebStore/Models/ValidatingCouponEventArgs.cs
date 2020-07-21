using System;
using System.Collections.Generic;
using HanumanInstitute.WebStore.Shared;

namespace HanumanInstitute.WebStore.Models
{
    /// <summary>
    /// Contains the parameters for the ValidatingCoupon event.
    /// </summary>
    public class ValidatingCouponEventArgs : EventArgs
    {
        public string? CouponCode { get; set; }
        public bool IsValid { get; set; }
        public string? Message { get; set; }

        public ValidatingCouponEventArgs(string couponCode)
        {
            CouponCode = couponCode;
        }
    }
}
