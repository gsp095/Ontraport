namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Processes payments via PayPal by submitting the sale via an Ontraport form.
    /// </summary>
    public interface IPayPalFormProcessor
    {
        /// <summary>
        /// Submits the sale via Ontraport form.
        /// </summary>
        string Submit(ProcessOrder order);
    }
}
