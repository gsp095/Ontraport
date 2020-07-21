using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HanumanInstitute.CommonWeb;
using HanumanInstitute.CommonWeb.Payments;
using HanumanInstitute.WebStore.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Res = HanumanInstitute.WebStore.Properties.Resources;

namespace HanumanInstitute.WebStore.Shared
{
    public partial class OrderForm : ComponentBase
    {
        [Inject, NotNull]
        public IStaticListsProvider? ListsProvider { get; set; }
        [Inject, NotNull]
        public IPaymentProcessor? Processor { get; set; }
        [Inject, NotNull]
        public NavigationManager? NavManager { get; set; }
        [Inject, NotNull]
        private ProductNames? productNames { get; set; }

        public OrderForm()
        { }

        public const string StrCreditCardUsd = "CreditCardUsd";
        public const string StrCreditCardCad = "CreditCardCad";
        public const string StrPayPalForm = "PayPalForm";
        public const string StrDefaultPaymentMethod = StrCreditCardUsd;
        public const string DefaultRedirectUrl = "https://store.spiritualselftransformation.com/confirm";

        protected override async void OnInitialized()
        {
            InputContext = new EditContext(Input);

            if (EnablePayPal && string.IsNullOrEmpty(OntraportFormId) && !LogOnly)
            {
                throw new ArgumentException(Res.FormIdNotSet);
            }

            await ValidateCouponCodeAsync().ConfigureAwait(false);

            base.OnInitialized();
        }

        public CouponModel Coupon { get; set; } = new CouponModel();
        public class CouponModel
        {
            public string? CouponCode { get; set; }
            public string? Message { get; set; }
            public string? Error { get; set; }
        }

        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            // Remove CreditCard object when not used so that it doesn't throw validation errors.
            public string PaymentMethod
            {
                get => _paymentMethod;
                set
                {
                    _paymentMethod = value;
                    if (value?.StartsWith("CreditCard", StringComparison.InvariantCulture) == true)
                    {
                        CreditCard = CreditCard ?? new ProcessOrderCreditCard();
                    }
                    else if (CreditCard != null)
                    {
                        CreditCard = null;
                    }
                }
            }
            private string _paymentMethod = StrDefaultPaymentMethod;
            [ValidateComplexType]
            public ProcessOrderCreditCard? CreditCard { get; set; } = new ProcessOrderCreditCard();
            [ValidateComplexType, Required]
            public ProcessOrderAddress Address { get; set; } = new ProcessOrderAddress();
            public string? Error { get; set; }
        }

        [NotNull]
        protected EditContext? InputContext { get; set; }

        protected bool ShowPayPal { get; set; } = true;
        protected bool ShowCreditCard { get; set; } = true;
        protected ListKeyValue<int?> ExpirationMonths => ListsProvider.ExpirationMonths;
        protected ListKeyValue<int?> ExpirationYears => ListsProvider.ExpirationYears;
        protected ListKeyValue States => ListsProvider.States;
        protected ListKeyValue Countries => ListsProvider.Countries;

        [Parameter]
        public EventCallback<OrderProcessingEventArgs> Processing { get; set; }
        [Parameter]
        public EventCallback<ValidatingCouponEventArgs> ValidatingCoupon { get; set; }

        [Parameter]
        [SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Reviewed: Configured via string attribute")]
        public string? ReturnUrl { get; set; }

        [Parameter]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Reviewed: Allow set via attribute")]
        public List<OrderFormProduct> Products { get; set; } = new List<OrderFormProduct>();

        [Parameter]
        public OrderFormProduct Product
        {
            get
            {
                if (!Products.Any())
                {
                    Products.Add(new OrderFormProduct());
                }
                return Products.First();
            }
            set
            {
                if (!Products.Any())
                {
                    Products.Add(value);
                }
                else
                {
                    Products[0] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Ontraport FormId is using PayPalForm payment method.
        /// </summary>
        [Parameter]
        public string OntraportFormId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether to allow PayPal as a payment method.
        /// </summary>
        [Parameter]
        public bool EnablePayPal { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to show the guarantee box.
        /// </summary>
        [Parameter]
        public string? Guarantee { get; set; }

        /// <summary>
        /// Gets or sets whether to only log transaction without processing payment.
        /// </summary>
        [Parameter]
        public bool LogOnly
        {
            get => _logOnly;
            set
            {
                _logOnly = value;
                Input.PaymentMethod = value ? string.Empty : StrDefaultPaymentMethod;
            }
        }
        private bool _logOnly;

        /// <summary>
        /// Gets or sets whether to display the order summary.
        /// </summary>
        [Parameter]
        public bool SummaryVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets the discount that was applied via coupons.
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Returns the total of the order.
        /// </summary>
        public decimal Total => Products.Cast<ProcessOrderProduct>().CalculateTotal();

        public async Task<string> GetConvertedCadTotalString()
        {
            var converted = await Processor.ConvertTotalAsync(Total, Currency.Cad).ConfigureAwait(false);
            return string.Format(CultureInfo.CreateSpecificCulture("en-CA"), "{0:C} CAD", converted);
        }

        public string GetProductText(OrderFormProduct product)
        {
            product.CheckNotNull(nameof(product));
            return product.Display ?? productNames.GetDisplayName(product.Name);
        }

        /// <summary>
        /// Validates and applies entered coupon code. Returns whether coupon code was valid.
        /// </summary>
        protected async Task<bool> ValidateCouponCodeAsync()
        {
            var result = true;
            Coupon.Message = string.Empty;
            Coupon.Error = string.Empty;
            Coupon.CouponCode = Coupon.CouponCode?.Trim() ?? string.Empty;
            var args = new ValidatingCouponEventArgs(Coupon.CouponCode);

            await ValidatingCoupon.InvokeAsync(args).ConfigureAwait(false);

            if (Coupon.CouponCode.Length > 0)
            {
                if (args.IsValid)
                {
                    Coupon.Message = args.Message ?? "Coupon code applied";
                    SetPayPalVisible(false);
                    if (GetPaymentMethod() == PaymentMethod.PayPalForm)
                    {
                        result = false; // Don't process order if PayPalForm is used with coupon.
                    }
                }
                else
                {
                    Coupon.Error = args.Message ?? "Invalid coupon code";
                    SetPayPalVisible(true);
                    result = false;
                }
            }
            else
            {
                SetPayPalVisible(EnablePayPal); // Reset EnablePayPal to its configured value
            }
            return result;
        }

        protected async Task CouponSubmitAsync() => await ValidateCouponCodeAsync().ConfigureAwait(false);

        protected async Task OrderSubmitAsync()
        {
            Input.Error = string.Empty;
            if (InputContext.Validate() && await ValidateCouponCodeAsync().ConfigureAwait(false))
            {
                var order = CreateOrder();
                await Processing.InvokeAsync(new OrderProcessingEventArgs(order)).ConfigureAwait(false);

                var result = await Processor.SubmitAsync(order).ConfigureAwait(false);

                if (result.Status == PaymentStatus.Declined)
                {
                    Input.Error = $"Payment Declined: {result.Message}";
                }

                if (result.Status == PaymentStatus.Approved)
                {
                    if (order.PaymentMethod == PaymentMethod.PayPalForm)
                    {
                        // Message contains the POST request HTML.
                        // return new TextActionResult(result.Message);
                    }
                    else
                    {
                        // Go to the confirmation page.
                        NavManager.NavigateTo(ReturnUrl ?? DefaultRedirectUrl, true);
                    }
                }
            }
        }

        public void RemoveProduct(OrderFormProduct item)
        {
            Products.Remove(item);
        }

        /// <summary>
        /// Sets the visibility of PayPal payment option.
        /// </summary>
        /// <param name="value">Whether PayPal option should be visible.</param>
        private void SetPayPalVisible(bool value)
        {
            ShowPayPal = value;
            if (!value)
            {
                ShowCreditCard = true;
                if (Input.PaymentMethod == StrPayPalForm)
                {
                    Input.PaymentMethod = StrDefaultPaymentMethod;
                }
            }
        }

        private ProcessOrder CreateOrder()
        {
            return new ProcessOrder()
            {
                CouponCode = Coupon.CouponCode,
                PaymentMethod = GetPaymentMethod(),
                PaymentCurrency = GetPaymentCurrency(),
                CreditCard = Input.CreditCard,
                Address = Input.Address,
                OntraportFormId = OntraportFormId
                // Discount = Discount
            }.SetProducts(Products.Select(x => x.CopyBase()));
        }

        private Currency GetPaymentCurrency() => Input.PaymentMethod == StrCreditCardCad ? Currency.Cad : Currency.Usd;

        private PaymentMethod GetPaymentMethod() => Input.PaymentMethod == StrPayPalForm ? PaymentMethod.PayPalForm : PaymentMethod.CreditCard;
    }
}
