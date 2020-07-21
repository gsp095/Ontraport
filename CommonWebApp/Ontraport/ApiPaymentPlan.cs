using System;
using HanumanInstitute.CommonWebApp.Ontraport;
using HanumanInstitute.OntraportApi.Converters;
using HanumanInstitute.OntraportApi.Models;

namespace HanumanInstitute.CommonWeb.Ontraport
{
    /// <summary>
    /// Payment plans represent purchases with recurring payments.
    /// </summary>
    public class ApiPaymentPlan : ApiCustomObjectBase
    {
        /// <summary>
        /// Returns a ApiProperty object to get or set the ID of the contact who owns this payment plan.
        /// </summary>
        public ApiProperty<int> ContactIdField => _contactIdField ??= new ApiProperty<int>(this, ContactIdKey);
        private ApiProperty<int>? _contactIdField;
        public const string ContactIdKey = "f1899";
        /// <summary>
        /// Gets or sets the ID of the contact who owns this payment plan.
        /// </summary>
        public int? ContactId { get => ContactIdField.Value; set => ContactIdField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the Ontraport product code being purchased.
        /// </summary>
        public ApiPropertyString ProductNameField => _productNameField ??= new ApiPropertyString(this, ProductNameKey);
        private ApiPropertyString? _productNameField;
        public const string ProductNameKey = "f1890";
        /// <summary>
        /// Gets or sets the Ontraport product code being purchased.
        /// </summary>
        public string? ProductName { get => ProductNameField.Value; set => ProductNameField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the date of the last recurring payment attempt.
        /// </summary>
        public ApiPropertyDateTime LastAttemptDateField => _lastAttemptDateField ??= new ApiPropertyDateTime(this, LastAttemptDateKey);
        private ApiPropertyDateTime? _lastAttemptDateField;
        public const string LastAttemptDateKey = "f1900";
        /// <summary>
        /// Gets or sets the date of the last recurring payment attempt.
        /// </summary>
        public DateTimeOffset? LastAttemptDate { get => LastAttemptDateField.Value; set => LastAttemptDateField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the date until which the customer has paid.
        /// </summary>
        public ApiPropertyDateTime PaidUntilDateField => _paidUntildDateField ??= new ApiPropertyDateTime(this, PaidUntilDateKey);
        private ApiPropertyDateTime? _paidUntildDateField;
        public const string PaidUntilDateKey = "f1910";
        /// <summary>
        /// Gets or sets the date until which the customer has paid.
        /// </summary>
        public DateTimeOffset? PaidUntilDate { get => PaidUntilDateField.Value; set => PaidUntilDateField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the amount to charge for each period.
        /// </summary>
        public ApiProperty<decimal> PricePerPeriodField => _pricePerPeriodField ??= new ApiProperty<decimal>(this, PricePerPeriodKey);
        private ApiProperty<decimal>? _pricePerPeriodField;
        public const string PricePerPeriodKey = "f1893";
        /// <summary>
        /// Gets or sets the amount to charge for each period.
        /// </summary>
        public decimal? PricePerPeriod { get => PricePerPeriodField.Value; set => PricePerPeriodField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the date of the next payment.
        /// </summary>
        public ApiPropertyDateTime NextChargeDateField => _nextChargeDateField ??= new ApiPropertyDateTime(this, NextChargeDateKey);
        private ApiPropertyDateTime? _nextChargeDateField;
        public const string NextChargeDateKey = "f1894";
        /// <summary>
        /// Gets or sets the date of the next payment.
        /// </summary>
        public DateTimeOffset? NextChargeDate { get => NextChargeDateField.Value; set => NextChargeDateField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the amount of transactions left to pay.
        /// </summary>
        public ApiProperty<int> TransactionsLeftField => _transactionsLeftField ??= new ApiProperty<int>(this, TransactionsLeftKey);
        private ApiProperty<int>? _transactionsLeftField;
        public const string TransactionsLeftKey = "f1895";
        /// <summary>
        /// Gets or sets the amount of transactions left to pay.
        /// </summary>
        public int? TransactionsLeft { get => TransactionsLeftField.Value; set => TransactionsLeftField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the total amount charged so far by this plan.
        /// </summary>
        public ApiProperty<decimal> TotalChargedField => _totalChargedField ??= new ApiProperty<decimal>(this, TotalChargedKey);
        private ApiProperty<decimal>? _totalChargedField;
        public const string TotalChargedKey = "f1896";
        /// <summary>
        /// Gets or sets the total amount charged so far by this plan.
        /// </summary>
        public decimal? TotalCharged { get => TotalChargedField.Value; set => TotalChargedField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the period unit between payments.
        /// </summary>
        public ApiPropertyStringEnum<PaymentPeriodUnit> PeriodUnitField => _periodUnitField ??= new ApiPropertyStringEnum<PaymentPeriodUnit>(this, PeriodUnitKey);
        private ApiPropertyStringEnum<PaymentPeriodUnit>? _periodUnitField;
        public const string PeriodUnitKey = "f1905";
        /// <summary>
        /// Gets or sets the period unit between payments.
        /// </summary>
        public PaymentPeriodUnit? PeriodUnit { get => PeriodUnitField.Value; set => PeriodUnitField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the quantity of period units between payments.
        /// </summary>
        public ApiProperty<int> PeriodQtyField => _periodQtyField ??= new ApiProperty<int>(this, PeriodQtyKey);
        private ApiProperty<int>? _periodQtyField;
        public const string PeriodQtyKey = "f1906";
        /// <summary>
        /// Gets or sets the quantity of period units between payments.
        /// </summary>
        public int? PeriodQty { get => PeriodQtyField.Value; set => PeriodQtyField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the start date of a subscription.
        /// </summary>
        public ApiPropertyDateTime StartDateField => _startDateField ??= new ApiPropertyDateTime(this, StartDateKey);
        private ApiPropertyDateTime? _startDateField;
        public const string StartDateKey = "f1907";
        /// <summary>
        /// Gets or sets the start date of a subscription.
        /// </summary>
        public DateTimeOffset? StartDate { get => StartDateField.Value; set => StartDateField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the end date of a subscription.
        /// </summary>
        public ApiPropertyDateTime EndDateField => _endDateField ??= new ApiPropertyDateTime(this, EndDateKey);
        private ApiPropertyDateTime? _endDateField;
        public const string EndDateKey = "f1908";
        /// <summary>
        /// Gets or sets the end date of a subscription.
        /// </summary>
        public DateTimeOffset? EndDate { get => EndDateField.Value; set => EndDateField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set whether a subscription was deleted and is pending payment.
        /// </summary>
        public ApiPropertyBool DeletedField => _deletedField ??= new ApiPropertyBool(this, DeletedKey);
        private ApiPropertyBool? _deletedField;
        public const string DeletedKey = "f1909";
        /// <summary>
        /// Gets or sets whether a subscirption was deleted and is pending payment.
        /// </summary>
        public bool? Deleted { get => DeletedField.Value; set => DeletedField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set a display description for the subscription.
        /// </summary>
        public ApiPropertyString DescriptiondField => _descriptiondField ??= new ApiPropertyString(this, DescriptiondKey);
        private ApiPropertyString? _descriptiondField;
        public const string DescriptiondKey = "f1933";
        /// <summary>
        /// Gets or sets a display description for the subscription.
        /// </summary>
        public string? Descriptiond { get => DescriptiondField.Value; set => DescriptiondField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set a picture file name associated with the subscription.
        /// </summary>
        public ApiPropertyString PictureFiledField => _pictureFiledField ??= new ApiPropertyString(this, PictureFiledKey);
        private ApiPropertyString? _pictureFiledField;
        public const string PictureFiledKey = "f1934";
        /// <summary>
        /// Gets or sets a picture file name associated with the subscription.
        /// </summary>
        public string? PictureFiled { get => PictureFiledField.Value; set => PictureFiledField.Value = value; }
    }
}
