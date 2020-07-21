using System;
using HanumanInstitute.CommonWeb.Payments;

namespace HanumanInstitute.WebStore.Models
{
    public class OrderProcessingEventArgs
    {
        public ProcessOrder Order { get; set; }

        public OrderProcessingEventArgs(ProcessOrder order)
        {
            Order = order;
        }
    }
}
