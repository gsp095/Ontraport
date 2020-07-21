using System;
using System.Collections.Specialized;
using System.Web;
using HanumanInstitute.CommonWeb;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Contains the response values from a BluePay transaction.
    /// </summary>
    public class BluePayResponse
    {
        public NameValueCollection Values { get; private set; }

        public BluePayResponse(string response)
        {
            response.CheckNotNullOrEmpty(nameof(response));
            var pos = response.IndexOf('?', StringComparison.InvariantCulture);
            if (pos > -1)
            {
                response = response.Substring(pos + 1);
            }
            Values = HttpUtility.ParseQueryString(response);
        }

        public string Status => Values["Result"];
        public string TransId => Values["RRNO"];
        public string Message => Values["MESSAGE"];
        public string CVV2 => Values["CVV2"];
        public string AVS => Values["AVS"];
        public string MaskedPaymentAccount => Values["PAYMENT_ACCOUNT"];
        public string CardType => Values["CARD_TYPE"];
        public string Bank => Values["BANK_NAME"];
        public string AuthCode => Values["AUTH_CODE"];
        public string RebillID => Values["REBID"];
        public string CreationDate => Values["creation_date"];
        public string NextDate => Values["next_date"];
        public string LastDate => Values["last_date"];
        public string SchedExpr => Values["sched_expr"];
        public string CyclesRemain => Values["cycles_remain"];
        public string RebillAmount => Values["reb_amount"];
        public string NextAmount => Values["next_amount"];
        public string CustomerToken => Values["CUST_TOKEN"];

        public bool IsSuccessfulTransaction => Status == "APPROVED" && Message != "DUPLICATE";
    }
}
