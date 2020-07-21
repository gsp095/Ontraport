/*
 * Bluepay C#.NET Sample code.
 *
 * Updated: 2017-01-19
 * https://github.com/BluePay/BluePay-Sample-Code/blob/master/C%23/BluePay.cs
 *
 * This code is Free.  You may use it, modify it and redistribute it.
 * If you do make modifications that are useful, Bluepay would love it if you donated
 * them back to us!
 *
 * 2020-05-10: Edited by Etienne Charland
 * 
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using static System.FormattableString;
using Res = HanumanInstitute.CommonWebApp.Properties.Resources;

namespace HanumanInstitute.CommonWeb.Payments
{
    /// <summary>
    /// Handles credit card payments through BluePay.
    /// </summary>
    public class BluePayProcessor : IBluePayProcessor
    {
        public const string UserAgent = "BluePay C# Library/3.0.2";

        // required for every transaction
        private string? _accountID;
        private Uri? _url;
        private string? _secretKey;
        private readonly string? _mode;
        private string? _api;

        // required for auth or sale
        private string? _paymentAccount;
        private string? _cvv2;
        private string? _cardExpire;
        private readonly Regex _track1And2 = new Regex(@"(%B)[\d\*]{0,19}\^([\w\s]*)\/([\w\s]*)([\s]*)\^[\d\*]{7}[\w*]*\?;[\d\*]{0,19}=[\d\*]{7}[\w*]*\?");
        private readonly Regex _track2Only = new Regex(@";[\d\*]{0,19}=[\d\*]{7}[\w*]*\?");
        private string? _swipeData;
        private string? _routingNum;
        private string? _accountNum;
        private string? _accountType;
        private string? _docType;
        private string? _firstName;
        private string? _lastName;
        private string? _addr1;
        private string? _city;
        private string? _state;
        private string? _zip;

        // optional for auth or sale
        private string? _addr2;
        private string? _phone;
        private string? _email;
        private string? _country;
        private string? _companyName;

        // transaction variables
        private string? _amount;
        private string? _transType;
        private string? _paymentType;
        private string? _masterID;
        private string? _rebillID;
        private string? _newCustToken;
        private string? _custToken;

        // rebill variables
        private string? _doRebill;
        private string? _rebillAmount;
        private string? _rebillFirstDate;
        private string? _rebillExpr;
        private string? _rebillCycles;
        private string? _rebillNextAmount;
        private string? _rebillNextDate;
        private string? _rebillStatus;
        private string? _templateID;

        // level2 variables
        private string? _customID1;
        private string? _customID2;
        private string? _invoiceID;
        private string? _orderID;
        private string? _amountTip;
        private string? _amountTax;
        private string? _amountFood;
        private string? _amountMisc;
        private string? _memo;

        // Generating Simple Hosted Payment Form URL fields
        private string? _dba;
        private Uri? _returnUrl;
        private string? _discoverImage;
        private string? _amexImage;
        private string? _protectAmount;
        private string? _rebillProtect;
        private string? _protectCustomID1;
        private string? _protectCustomID2;
        private string? _shpfFormID;
        private string? _receiptFormID;
        private Uri? _remoteUrl;
        private string? _shpfTpsHashType;
        private string? _receiptTpsHashType;
        private string? _cardTypes;
        private string? _receiptTpsDef;
        private string? _receiptTpsString;
        private string? _receiptTamperProofSeal;
        private Uri? _receiptUrl;
        private string? _bp10emuTpsDef;
        private string? _bp10emuTpsString;
        private string? _bp10emuTamperProofSeal;
        private string? _shpfTpsDef;
        private string? _shpfTpsString;
        private string? _shpfTamperProofSeal;

        // rebill fields
        private string? _reportStartDate;
        private string? _reportEndDate;
        private string? _doNotEscape;
        private string? _queryBySettlement;
        private string? _queryByHierarchy;
        private string? _excludeErrors;

        private string? _tps;

        private string _tpsHashType = "HMAC_SHA512";

        // level 2 processing field
        private readonly Dictionary<string, string?> _level2Info = new Dictionary<string, string?>();

        // level 3 processing field
        private readonly List<Dictionary<string, string?>> _lineItems = new List<Dictionary<string, string?>>();

        private readonly HttpClient _httpClient;
        private readonly IWebEnvironment _environment;
        private readonly IRandomGenerator _random;
        private readonly IOptions<BluePayConfig> _config;

        public BluePayProcessor(HttpClient httpClient, IWebEnvironment environment, IRandomGenerator random, IOptions<BluePayConfig> config)
        {
            _httpClient = httpClient.CheckNotNull(nameof(httpClient));
            _environment = environment.CheckNotNull(nameof(environment));
            _random = random;
            _config = config.CheckNotNull(nameof(config));

            _mode = _config.Value.TestMode ? "TEST" : "LIVE";
            SetCurrency(Currency.Usd);
        }

        /// <summary>
        /// Sets the currency to use for transactions. Must be called before using this class.
        /// </summary>
        /// <param name="currency">The currency to use for trarnsactions.</param>
        public void SetCurrency(Currency currency)
        {
            if (currency == Currency.Cad)
            {
                _accountID = _config.Value.AccountIdCad;
                _secretKey = _config.Value.SecretKeyCad;
            }
            else if (currency == Currency.Usd)
            {
                _accountID = _config.Value.AccountIdUsd;
                _secretKey = _config.Value.SecretKeyUsd;
            }
            else
            {
                throw new ArgumentException(Res.BluePayInvalidCurrency);
            }
        }

        /// <summary>
        /// Sets Customer Information
        /// </summary>
        public void SetCustomerInformation(string firstName, string lastName, string? address1 = null, string? address2 = null, string? city = null, string? state = null, string? zip = null, string? country = null, string? phone = null, string? email = null, string? companyName = null)
        {
            _firstName = firstName;
            _lastName = lastName;
            _addr1 = address1;
            _addr2 = address2;
            _city = city;
            _state = state;
            _zip = zip;
            _country = country;
            _phone = phone;
            _email = email;
            _companyName = companyName;
        }

        /// <summary>
        /// Sets Credit Card Information
        /// </summary>
        public void SetCCInformation(string ccNumber, int expMonth, int expYear, string cvv2)
        {
            if (expMonth < 1 || expMonth > 12) { throw new ArgumentException(Res.CreditCardExpirationMonthInvalid.FormatInvariant(expMonth)); }
            if (expYear < 0 || expYear > 99) { throw new ArgumentException(Res.CreditCardExpirationYearInvalid.FormatInvariant(expYear)); }

            _paymentType = "CREDIT";
            _paymentAccount = ccNumber;
            _cardExpire = Invariant($"{expMonth:00}{expYear:00}");
            _cvv2 = cvv2;
        }

        /// <summary>
        /// Sets Level2 variables.
        /// </summary>
        public void SetLevel2Information(string? customID1, string? customID2, string? invoiceID, string? orderID, string? amountTip, string? amountTax, string? amountFood, string? amountMisc, string? memo)
        {
            _customID1 = customID1;
            _customID2 = customID2;
            _invoiceID = invoiceID;
            _orderID = orderID;
            _amountTip = amountTip;
            _amountTax = amountTax;
            _amountFood = amountFood;
            _amountMisc = amountMisc;
            _memo = memo;
        }

        /// <summary>
        /// Sets Swipe Information Using Either Both Track 1 2, Or Just Track 2
        /// </summary>
        public void Swipe(string swipe)
        {
            _paymentType = "CREDIT";
            _swipeData = swipe;
        }

        /// <summary>
        /// Sets ACH Information
        /// </summary>
        public void SetACHInformation(string routingNum, string accountNum, string accountType, string? docType = null)
        {
            _paymentType = "ACH";
            _routingNum = routingNum;
            _accountNum = accountNum;
            _accountType = accountType;
            _docType = docType;
        }

        /// <summary>
        /// Adds information required for level 2 processing.
        /// </summary>
        public void AddLevel2Information(string? taxRate = null, string? goodsTaxRate = null, string? goodsTaxAmount = null, string? shippingAmount = null, string? discountAmount = null, string? custPO = null, string? goodsTaxID = null, string? taxID = null, string? customerTaxID = null, string? dutyAmount = null, string? supplementalData = null, string? cityTaxRate = null, string? cityTaxAmount = null, string? countyTaxRate = null, string? countyTaxAmount = null, string? stateTaxRate = null, string? stateTaxAmount = null, string? buyerName = null, string? customerReference = null, string? customerNumber = null, string? shipName = null, string? shipAddr1 = null, string? shipAddr2 = null, string? shipCity = null, string? shipState = null, string? shipZip = null, string? shipCountry = null)
        {
            _level2Info.Add("LV2_ITEM_TAX_RATE", taxRate);
            _level2Info.Add("LV2_ITEM_GOODS_TAX_RATE", goodsTaxRate);
            _level2Info.Add("LV2_ITEM_GOODS_TAX_AMOUNT", goodsTaxAmount);
            _level2Info.Add("LV2_ITEM_SHIPPING_AMOUNT", shippingAmount);
            _level2Info.Add("LV2_ITEM_DISCOUNT_AMOUNT", discountAmount);
            _level2Info.Add("LV2_ITEM_CUST_PO", custPO);
            _level2Info.Add("LV2_ITEM_GOODS_TAX_ID", goodsTaxID);
            _level2Info.Add("LV2_ITEM_TAX_ID", taxID);
            _level2Info.Add("LV2_ITEM_CUSTOMER_TAX_ID", customerTaxID);
            _level2Info.Add("LV2_ITEM_DUTY_AMOUNT", dutyAmount);
            _level2Info.Add("LV2_ITEM_SUPPLEMENTAL_DATA", supplementalData);
            _level2Info.Add("LV2_ITEM_CITY_TAX_RATE", cityTaxRate);
            _level2Info.Add("LV2_ITEM_CITY_TAX_AMOUNT", cityTaxAmount);
            _level2Info.Add("LV2_ITEM_COUNTY_TAX_RATE", countyTaxRate);
            _level2Info.Add("LV2_ITEM_COUNTY_TAX_AMOUNT", countyTaxAmount);
            _level2Info.Add("LV2_ITEM_STATE_TAX_RATE", stateTaxRate);
            _level2Info.Add("LV2_ITEM_STATE_TAX_AMOUNT", stateTaxAmount);
            _level2Info.Add("LV2_ITEM_BUYER_NAME", buyerName);
            _level2Info.Add("LV2_ITEM_CUSTOMER_REFERENCE", customerReference);
            _level2Info.Add("LV2_ITEM_CUSTOMER_NUMBER", customerNumber);
            _level2Info.Add("LV2_ITEM_SHIP_NAME", shipName);
            _level2Info.Add("LV2_ITEM_SHIP_ADDR1", shipAddr1);
            _level2Info.Add("LV2_ITEM_SHIP_ADDR2", shipAddr2);
            _level2Info.Add("LV2_ITEM_SHIP_CITY", shipCity);
            _level2Info.Add("LV2_ITEM_SHIP_STATE", shipState);
            _level2Info.Add("LV2_ITEM_SHIP_ZIP", shipZip);
            _level2Info.Add("LV2_ITEM_SHIP_COUNTRY", shipCountry);
        }

        /// <summary>
        /// Adds a line item for level 3 processing. Repeat method for each item up to 99 items.
        /// For Canadian and AMEX processors, ensure required Level 2 information is present.
        /// </summary>
        public void AddLineItem(string? unitCost, string? quantity, string? itemSKU = null, string? descriptor = null, string? commodityCode = null, string? productCode = null, string? measureUnits = null, string? itemDiscount = null, string? taxRate = null, string? goodsTaxRate = null, string? taxAmount = null, string? goodsTaxAmount = null, string? cityTaxRate = null, string? cityTaxAmount = null, string? countyTaxRate = null, string? countyTaxAmount = null, string? stateTaxRate = null, string? stateTaxAmount = null, string? custSKU = null, string? custPO = null, string? supplementalData = null, string? glAccountNumber = null, string? divisionNumber = null, string? poLineNumber = null, string? lineItemTotal = null)
        {
            var i = _lineItems.Count + 1;
            var prefix = $"LV3_ITEM{i}_";
            var item = new Dictionary<string, string?>
            {
                { prefix + "UNIT_COST", unitCost },
                { prefix + "QUANTITY", quantity },
                { prefix + "ITEM_SKU", itemSKU },
                { prefix + "ITEM_DESCRIPTOR", descriptor },
                { prefix + "COMMODITY_CODE", commodityCode },
                { prefix + "PRODUCT_CODE", productCode },
                { prefix + "MEASURE_UNITS", measureUnits },
                { prefix + "ITEM_DISCOUNT", itemDiscount },
                { prefix + "TAX_RATE", taxRate },
                { prefix + "GOODS_TAX_RATE", goodsTaxRate },
                { prefix + "TAX_AMOUNT", taxAmount },
                { prefix + "GOODS_TAX_AMOUNT", goodsTaxAmount },
                { prefix + "CITY_TAX_RATE", cityTaxRate },
                { prefix + "CITY_TAX_AMOUNT", cityTaxAmount },
                { prefix + "COUNTY_TAX_RATE", countyTaxRate },
                { prefix + "COUNTY_TAX_AMOUNT", countyTaxAmount },
                { prefix + "STATE_TAX_RATE", stateTaxRate },
                { prefix + "STATE_TAX_AMOUNT", stateTaxAmount },
                { prefix + "CUST_SKU", custSKU },
                { prefix + "CUST_PO", custPO },
                { prefix + "SUPPLEMENTAL_DATA", supplementalData },
                { prefix + "GL_ACCOUNT_NUMBER", glAccountNumber },
                { prefix + "DIVISION_NUMBER", divisionNumber },
                { prefix + "PO_LINE_NUMBER", poLineNumber },
                { prefix + "LINE_ITEM_TOTAL", lineItemTotal }
            };
            _lineItems.Add(item);
        }

        /// <summary>
        /// Sets Rebilling Cycle Information. To be used with other functions to create a transaction.
        /// </summary>
        public void SetRebillingInformation(string rebAmount, string rebFirstDate, string rebExpr, string rebCycles)
        {
            _doRebill = "1";
            _rebillFirstDate = rebFirstDate;
            _rebillExpr = rebExpr;
            _rebillCycles = rebCycles;
            _rebillAmount = rebAmount;
        }

        /// <summary>
        /// Updates Rebilling Cycle
        /// </summary>
        public void UpdateRebillingInformation(string rebillID, string rebNextDate, string rebExpr, string rebCycles, string rebAmount, string rebNextAmount, string? templateID = null)
        {
            _transType = "SET";
            _api = "bp20rebadmin";
            _rebillID = rebillID;
            _rebillNextDate = rebNextDate;
            _rebillExpr = rebExpr;
            _rebillCycles = rebCycles;
            _rebillAmount = rebAmount;
            _rebillNextAmount = rebNextAmount;
            _templateID = templateID;
        }

        /// <summary>
        /// Updates a rebilling cycle's payment information
        /// </summary>
        public void UpdateRebillPaymentInformation(string templateID) => _templateID = templateID;

        /// <summary>
        /// Cancels Rebilling Cycle
        /// </summary>
        public void CancelRebilling(string rebillID)
        {
            _transType = "SET";
            _rebillStatus = "stopped";
            _rebillID = rebillID;
            _api = "bp20rebadmin";
        }

        /// <summary>
        /// Gets a existing rebilling cycle's status
        /// </summary>
        public void GetRebillStatus(string rebillID)
        {
            _transType = "GET";
            _rebillID = rebillID;
            _api = "bp20rebadmin";
        }

        /// <summary>
        /// Gets Report of Transaction Data
        /// </summary>
        public void GetTransactionReport(string reportStartDate, string reportEndDate, string subaccountsSearched, string? doNotEscape = null, string? excludeErrors = null)
        {
            _queryBySettlement = "0";
            _reportStartDate = reportStartDate;
            _reportEndDate = reportEndDate;
            _queryByHierarchy = subaccountsSearched;
            _doNotEscape = doNotEscape;
            _excludeErrors = excludeErrors;
            _api = "bpdailyreport2";
        }

        /// <summary>
        /// Gets Report of Settled Transaction Data
        /// </summary>
        public void GetTransactionSettledReport(string reportStartDate, string reportEndDate, string subaccountsSearched, string? doNotEscape = null, string? excludeErrors = null)
        {
            _queryBySettlement = "1";
            _reportStartDate = reportStartDate;
            _reportEndDate = reportEndDate;
            _queryByHierarchy = subaccountsSearched;
            _doNotEscape = doNotEscape;
            _excludeErrors = excludeErrors;
            _api = "bpdailyreport2";
        }

        /// <summary>
        /// Gets Details of a Transaction
        /// </summary>
        /// <param name="transactionID">Queries by Transaction ID.</param>
        /// <param name="reportStartDate">The date from which to retrieve transactions data.</param>
        /// <param name="reportEndDate">The date until which to retrieve transactions data.</param>
        /// <param name="excludeErrors"></param>
        /// <param name="paymentType">Queries by Payment Type.</param>
        /// <param name="transType">Queries by Transaction Type.</param>
        /// <param name="amount">Queries by Transaction Amount.</param>
        /// <param name="firstName">Queries by First Name (NAME1).</param>
        /// <param name="lastName">Queries by Last Name (NAME2).</param>
        public void GetSingleTransQuery(string transactionID, string reportStartDate, string reportEndDate, string? excludeErrors = null, string? paymentType = null, string? transType = null, decimal? amount = null, string? firstName = null, string? lastName = null)
        {
            _reportStartDate = reportStartDate;
            _reportEndDate = reportEndDate;
            _excludeErrors = excludeErrors;
            _masterID = transactionID;
            _api = "stq";
            _paymentType = paymentType;
            _transType = transType;
            _amount = amount?.ToString("0.00", CultureInfo.InvariantCulture);
            _firstName = firstName;
            _lastName = lastName;
        }

        /// <summary>
        /// Runs a Sale Transaction
        /// </summary>
        public void Sale(decimal amount, string? masterID = null, string? customerToken = null)
        {
            _transType = "SALE";
            _api = "bp10emu";
            _amount = amount.ToString("0.00", CultureInfo.InvariantCulture);
            _masterID = masterID;
            _custToken = customerToken;
        }

        /// <summary>
        /// Runs an Auth Transaction
        /// </summary>
        public void Auth(decimal amount, string? masterID = null, string? newCustomerToken = null, string? customerToken = null)
        {
            _transType = "AUTH";
            _api = "bp10emu";
            _amount = amount.ToString("0.00", CultureInfo.InvariantCulture);
            _masterID = masterID;
            if (!string.IsNullOrEmpty(newCustomerToken) && newCustomerToken.ToUpperInvariant() != "FALSE")
            {
                _newCustToken = newCustomerToken.ToUpperInvariant() == "TRUE" ? _random.GetSecureToken(16) : newCustomerToken;
            }
            _custToken = customerToken;
        }

        /// <summary>
        /// Runs a Refund Transaction
        /// </summary>
        public void Refund(string masterID, decimal? amount = null)
        {
            _transType = "REFUND";
            _api = "bp10emu";
            _masterID = masterID;
            _amount = amount?.ToString("0.00", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Updates a Transaction
        /// </summary>
        public void Update(string masterID, decimal? amount = null)
        {
            _transType = "UPDATE";
            _api = "bp10emu";
            _masterID = masterID;
            _amount = amount?.ToString("0.00", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Voids a transaction
        /// </summary>
        public void Void(string masterID)
        {
            _transType = "VOID";
            _api = "bp10emu";
            _masterID = masterID;
        }

        /// <summary>
        /// Runs a Capture Transaction
        /// </summary>
        public void Capture(string masterID, decimal? amount = null)
        {
            _transType = "CAPTURE";
            _masterID = masterID;
            _amount = amount?.ToString("0.00", CultureInfo.InvariantCulture);
            _api = "bp10emu";
        }

        /// <summary>
        /// Calculates TAMPER_PROOF_SEAL for bp20post API
        /// </summary>
        private void CalcTPS()
        {
            var tamper_proof_seal = _accountID
                                    + _transType
                                    + _amount
                                    + _doRebill
                                    + _rebillFirstDate
                                    + _rebillExpr
                                    + _rebillCycles
                                    + _rebillAmount
                                    + _masterID
                                    + _mode;
            _tps = GenerateTPS(tamper_proof_seal, _tpsHashType);
        }

        /// <summary>
        /// Calculates TAMPER_PROOF_SEAL for bp20rebadmin API
        /// </summary>
        private void CalcRebillTPS()
        {
            var tamper_proof_seal = _accountID + _transType + _rebillID;
            _tps = GenerateTPS(tamper_proof_seal, _tpsHashType);
        }

        /// <summary>
        /// Calculates TAMPER_PROOF_SEAL for bpdailyreport2 and stq APIs
        /// </summary>
        private void CalcReportTPS()
        {
            var tamper_proof_seal = _accountID + _reportStartDate + _reportEndDate;
            _tps = GenerateTPS(tamper_proof_seal, _tpsHashType);
        }

        /// <summary>
        /// Generates the TAMPER_PROOF_SEAL to used to validate each transaction
        /// </summary>
        private string GenerateTPS(string message, string hashType)
        {
            if (_secretKey == null)
            {
                return "SECRET KEY NOT PROVIDED";
            }

            string? tpsHash = null;
            var encode = new ASCIIEncoding();

            if (hashType == "HMAC_SHA256")
            {
                var secretKeyBytes = encode.GetBytes(_secretKey);
                var messageBytes = encode.GetBytes(message);
                using var hmac = new HMACSHA256(secretKeyBytes);
                var hashBytes = hmac.ComputeHash(messageBytes);
                tpsHash = ByteArrayToString(hashBytes);
            }
            else if (hashType == "SHA512")
            {
                using SHA512 sha512 = new SHA512CryptoServiceProvider();
                var buffer = encode.GetBytes(_secretKey + message);
                var hash = sha512.ComputeHash(buffer);
                tpsHash = ByteArrayToString(hash);
            }
            else if (hashType == "SHA256")
            {
                using SHA256 sha256 = new SHA256CryptoServiceProvider();
                var buffer = encode.GetBytes(_secretKey + message);
                var hash = sha256.ComputeHash(buffer);
                tpsHash = ByteArrayToString(hash);
            }
            //else if (HashType == "MD5")
            //{
            //    using MD5 md5 = new MD5CryptoServiceProvider();
            //    var buffer = encode.GetBytes(_secretKey + Message);
            //    var hash = md5.ComputeHash(buffer);
            //    tpsHash = ByteArrayToString(hash);
            //}
            else
            {
                var secretKeyBytes = encode.GetBytes(_secretKey);
                var messageBytes = encode.GetBytes(message);
                using var hmac = new HMACSHA512(secretKeyBytes);
                var hashBytes = hmac.ComputeHash(messageBytes);
                tpsHash = ByteArrayToString(hashBytes);
            }

            return tpsHash;
        }

        //This is used to convert a byte array to a hex string
        private static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            var sOutput = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2", CultureInfo.InvariantCulture));
            }
            return sOutput.ToString();
        }

        /// <summary>
        /// Calls the methods necessary to generate a SHPF URL
        /// 
        /// Template IDs:
        /// mobileform01 -- Credit Card Only - White Vertical (mobile capable) 
        /// default1v5 -- Credit Card Only - Gray Horizontal 
        /// default7v5 -- Credit Card Only - Gray Horizontal Donation
        /// default7v5R -- Credit Card Only - Gray Horizontal Donation with Recurring
        /// default3v4 -- Credit Card Only - Blue Vertical with card swipe
        /// mobileform02 -- Credit Card & ACH - White Vertical (mobile capable)
        /// default8v5 -- Credit Card & ACH - Gray Horizontal Donation
        /// default8v5R -- Credit Card & ACH - Gray Horizontal Donation with Recurring
        /// mobileform03 -- ACH Only - White Vertical (mobile capable)
        /// mobileresult01 -- Default without signature line - White Responsive (mobile)
        /// defaultres1 -- Default without signature line â€“ Blue
        /// V5results -- Default without signature line â€“ Gray
        /// V5Iresults -- Default without signature line â€“ White
        /// defaultres2 -- Default with signature line â€“ Blue
        /// remote_url - Use a remote URL
        /// </summary>
        /// <param name="merchantName">Merchant name that will be displayed in the payment page.</param>
        /// <param name="returnURL">Link to be displayed on the transacton results page. Usually the merchant's web site home page.</param>
        /// <param name="transactionType">SALE/AUTH -- Whether the customer should be charged or only check for enough credit available.</param>
        /// <param name="acceptDiscover">Yes/No -- Yes for most US merchants. No for most Canadian merchants.</param>
        /// <param name="acceptAmex">Yes/No -- Has an American Express merchant account been set up?</param>
        /// <param name="amount">The amount if the merchant is setting the initial amount.</param>
        /// <param name="protectAmount">Yes/No -- Should the amount be protected from changes by the tamperproof seal?</param>
        /// <param name="rebilling">Yes/No -- Should a recurring transaction be set up?</param>
        /// <param name="rebProtect">Yes/No -- Should the rebilling fields be protected by the tamperproof seal?</param>
        /// <param name="rebAmount">Amount that will be charged when a recurring transaction occurs.</param>
        /// <param name="rebCycles">Number of times that the recurring transaction should occur. Not set if recurring transactions should continue until canceled.</param>
        /// <param name="rebStartDate">Date (yyyy-mm-dd) or period (x units) until the first recurring transaction should occur. Possible units are DAY, DAYS, WEEK, WEEKS, MONTH, MONTHS, YEAR or YEARS. (ex. 2016-04-01 or 1 MONTH)</param>
        /// <param name="rebFrequency">How often the recurring transaction should occur. Format is 'X UNITS'. Possible units are DAY, DAYS, WEEK, WEEKS, MONTH, MONTHS, YEAR or YEARS. (ex. 1 MONTH) </param>
        /// <param name="customID1">A merchant defined custom ID value.</param>
        /// <param name="protectCustomID1">Yes/No -- Should the Custom ID value be protected from change using the tamperproof seal?</param>
        /// <param name="customID2">A merchant defined custom ID 2 value.</param>
        /// <param name="protectCustomID2">Yes/No -- Should the Custom ID 2 value be protected from change using the tamperproof seal?</param>
        /// <param name="paymentTemplate">Select one of our payment form template IDs or your own customized template ID. If the customer should not be allowed to change the amount, add a 'D' to the end of the template ID. Example: 'mobileform01D'</param>
        /// <param name="receiptTemplate">Select one of our receipt form template IDs, your own customized template ID, or "remote_url" if you have one.</param>
        /// <param name="receiptTempRemoteURL">Your remote URL ** Only required if receiptTemplate = "remote_url".</param>
        public Uri GenerateURL(string merchantName, Uri? returnUrl, string transactionType, string acceptDiscover, string acceptAmex, decimal? amount = null, string? protectAmount = "No", string? rebilling = "No", string? rebProtect = "Yes", string? rebAmount = null, string? rebCycles = null, string? rebStartDate = null, string? rebFrequency = null, string? customID1 = null, string? protectCustomID1 = "No", string? customID2 = null, string? protectCustomID2 = "No", string? paymentTemplate = "mobileform01", string? receiptTemplate = "mobileresult01", Uri? receiptTempRemoteUrl = null)
        {
            _dba = merchantName;
            _returnUrl = returnUrl;
            _transType = transactionType;
            _discoverImage = acceptDiscover?.ToUpperInvariant()[0] == 'Y' ? "discvr.gif" : "spacer.gif";
            _amexImage = acceptAmex?.ToUpperInvariant()[0] == 'Y' ? "amex.gif" : "spacer.gif";
            _amount = amount?.ToString("0.00", CultureInfo.InvariantCulture);
            _protectAmount = protectAmount;
            _doRebill = rebilling?.ToUpperInvariant()[0] == 'Y' ? "1" : "0";
            _rebillProtect = rebProtect;
            _rebillAmount = rebAmount;
            _rebillCycles = rebCycles;
            _rebillFirstDate = rebStartDate;
            _rebillExpr = rebFrequency;
            _customID1 = customID1;
            _protectCustomID1 = protectCustomID1;
            _customID2 = customID2;
            _protectCustomID2 = protectCustomID2;
            _shpfFormID = paymentTemplate;
            _receiptFormID = receiptTemplate;
            _remoteUrl = receiptTempRemoteUrl;
            _shpfTpsHashType = "HMAC_SHA512";
            _receiptTpsHashType = _shpfTpsHashType;
            _tpsHashType = SetHashType(_tpsHashType);
            _cardTypes = SetCardTypes();
            _receiptTpsDef = "SHPF_ACCOUNT_ID SHPF_FORM_ID RETURN_URL DBA AMEX_IMAGE DISCOVER_IMAGE SHPF_TPS_DEF SHPF_TPS_HASH_TYPE";
            _receiptTpsString = SetReceiptTpsString();
            _receiptTamperProofSeal = GenerateTPS(_receiptTpsString, _receiptTpsHashType);
            _receiptUrl = SetReceiptUrl();
            _bp10emuTpsDef = AddDefProtectedStatus("MERCHANT APPROVED_URL DECLINED_URL MISSING_URL MODE TRANSACTION_TYPE TPS_DEF TPS_HASH_TYPE");
            _bp10emuTpsString = SetBp10emuTpsString();
            _bp10emuTamperProofSeal = GenerateTPS(_bp10emuTpsString, _tpsHashType);
            _shpfTpsDef = AddDefProtectedStatus("SHPF_FORM_ID SHPF_ACCOUNT_ID DBA TAMPER_PROOF_SEAL AMEX_IMAGE DISCOVER_IMAGE TPS_DEF TPS_HASH_TYPE SHPF_TPS_DEF SHPF_TPS_HASH_TYPE");
            _shpfTpsString = SetShpfTpsString();
            _shpfTamperProofSeal = GenerateTPS(_shpfTpsString, _shpfTpsHashType);
            return CalcResponseUrl();
        }

        /// <summary>
        /// Validates chosen hash type or returns default hash.
        /// </summary>
        private static string SetHashType(string chosenHash)
        {
            const string DefaultHash = "HMAC_SHA512";
            chosenHash = chosenHash?.ToUpperInvariant() ?? string.Empty;
            string[] hashes = { "MD5", "SHA256", "SHA512", "HMAC_SHA256" };
            return hashes.Contains(chosenHash) ? chosenHash : DefaultHash;
        }

        /// <summary>
        /// Sets the types of credit card images to use on the Simple Hosted Payment Form. Must be used with GenerateURL.
        /// </summary>
        private string SetCardTypes()
        {
            var creditCards = "vi-mc";
            creditCards = _discoverImage == "discvr.gif" ? (creditCards + "-di") : creditCards;
            creditCards = _amexImage == "amex.gif" ? (creditCards + "-am") : creditCards;
            return creditCards;
        }

        /// <summary>
        /// Sets the receipt Tamperproof Seal string. Must be used with GenerateURL.
        /// </summary>
        private string SetReceiptTpsString()
        {
            return _accountID + _receiptFormID + _returnUrl + _dba + _amexImage + _discoverImage + _receiptTpsDef + _receiptTpsHashType;
        }

        /// <summary>
        /// Sets the bp10emu string that will be used to create a Tamperproof Seal. Must be used with GenerateURL.
        /// </summary>
        private string SetBp10emuTpsString()
        {
            var bp10emu = _accountID + _receiptUrl + _receiptUrl + _receiptUrl + _mode + _transType + _bp10emuTpsDef + _tpsHashType;
            return AddStringProtectedStatus(bp10emu);
        }

        /// <summary>
        /// Sets the Simple Hosted Payment Form string that will be used to create a Tamperproof Seal. Must be used with GenerateURL.
        /// </summary>
        private string SetShpfTpsString()
        {
            var shpf = _shpfFormID + _accountID + _dba + _bp10emuTamperProofSeal + _amexImage + _discoverImage + _bp10emuTpsDef + _tpsHashType + _shpfTpsDef + _shpfTpsHashType;
            return AddStringProtectedStatus(shpf);
        }

        /// <summary>
        /// Sets the receipt url or uses the remote url provided. Must be used with GenerateURL.
        /// </summary>
        private Uri SetReceiptUrl()
        {
            if (_receiptFormID == "remote_url" && _remoteUrl != null)
            {
                return _remoteUrl;
            }
            else
            {
                var url = new QueryStringBuilder("https://secure.bluepay.com/interfaces/shpf?")
                    .AppendKeyValue("SHPF_FORM_ID", _receiptFormID)
                    .AppendKeyValue("SHPF_ACCOUNT_ID", _accountID)
                    .AppendKeyValue("SHPF_TPS_DEF", _receiptTpsDef)
                    .AppendKeyValue("SHPF_TPS_HASH_TYPE", _receiptTpsHashType)
                    .AppendKeyValue("SHPF_TPS", _receiptTamperProofSeal)
                    .AppendKeyValue("RETURN_URL", _returnUrl?.AbsoluteUri)
                    .AppendKeyValue("DBA", _dba)
                    .AppendKeyValue("AMEX_IMAGE", _amexImage)
                    .AppendKeyValue("DISCOVER_IMAGE", _discoverImage);
                return new Uri(url.ToString());
            }
        }

        /// <summary>
        /// Adds optional protected keys to a string. Must be used with GenerateURL.
        /// </summary>
        private string AddDefProtectedStatus(string input)
        {
            if (_protectAmount == "Yes")
            {
                input += " AMOUNT";
            }

            if (_rebillProtect == "Yes")
            {
                input += " REBILLING REB_CYCLES REB_AMOUNT REB_EXPR REB_FIRST_DATE";
            }

            if (_protectCustomID1 == "Yes")
            {
                input += " CUSTOM_ID";
            }

            if (_protectCustomID2 == "Yes")
            {
                input += " CUSTOM_ID2";
            }

            return input;
        }

        /// <summary>
        /// Adds optional protected values to a string. Must be used with GenerateURL.
        /// </summary>
        private string AddStringProtectedStatus(string input)
        {
            if (_protectAmount == "Yes")
            {
                input += _amount;
            }

            if (_rebillProtect == "Yes")
            {
                input += _doRebill + _rebillCycles + _rebillAmount + _rebillExpr + _rebillFirstDate;
            }

            if (_protectCustomID1 == "Yes")
            {
                input += _customID1;
            }

            if (_protectCustomID2 == "Yes")
            {
                input += _customID2;
            }

            return input;
        }

        /// <summary>
        /// Generates the final url for the Simple Hosted Payment Form. Must be used with GenerateURL.
        /// </summary>
        private Uri CalcResponseUrl()
        {
            var url = new QueryStringBuilder("https://secure.bluepay.com/interfaces/shpf?")
                .AppendKeyValue("SHPF_FORM_ID", _shpfFormID)
                .AppendKeyValue("SHPF_ACCOUNT_ID", _accountID)
                .AppendKeyValue("SHPF_TPS_DEF", _shpfTpsDef)
                .AppendKeyValue("SHPF_TPS_HASH_TYPE", _shpfTpsHashType)
                .AppendKeyValue("SHPF_TPS", _shpfTamperProofSeal)
                .AppendKeyValue("MODE", _mode)
                .AppendKeyValue("TRANSACTION_TYPE", _transType)
                .AppendKeyValue("DBA", _dba)
                .AppendKeyValue("AMOUNT", _amount)
                .AppendKeyValue("TAMPER_PROOF_SEAL", _bp10emuTamperProofSeal)
                .AppendKeyValue("CUSTOM_ID", _customID1)
                .AppendKeyValue("CUSTOM_ID2", _customID2)
                .AppendKeyValue("REBILLING", _doRebill)
                .AppendKeyValue("REB_CYCLES", _rebillCycles)
                .AppendKeyValue("REB_AMOUNT", _rebillAmount)
                .AppendKeyValue("REB_EXPR", _rebillExpr)
                .AppendKeyValue("REB_FIRST_DATE", _rebillFirstDate)
                .AppendKeyValue("AMEX_IMAGE", _amexImage)
                .AppendKeyValue("DISCOVER_IMAGE", _discoverImage)
                .AppendKeyValue("REDIRECT_URL", _receiptUrl?.AbsoluteUri)
                .AppendKeyValue("TPS_DEF", _bp10emuTpsDef)
                .AppendKeyValue("TPS_HASH_TYPE", _tpsHashType)
                .AppendKeyValue("CARD_TYPES", _cardTypes);

            return new Uri(url.ToString(), UriKind.Absolute);
        }

        /// <summary>
        /// Posts the transaction to the server.
        /// </summary>
        /// <returns>The transaction result.</returns>
        public async Task<BluePayResponse> ProcessAsync()
        {
            var postData = new QueryStringBuilder();

            switch (_api)
            {
                // A switch section can have more than one case label. 
                case "bpdailyreport2":
                    CalcReportTPS();
                    _url = new Uri("https://secure.bluepay.com/interfaces/bpdailyreport2", UriKind.Absolute);
                    postData.AppendKeyValue("ACCOUNT_ID", _accountID)
                        .AppendKeyValue("MODE", _mode)
                        .AppendKeyValue("TAMPER_PROOF_SEAL", _tps)
                        .AppendKeyValue("REPORT_START_DATE", _reportStartDate)
                        .AppendKeyValue("REPORT_END_DATE", _reportEndDate)
                        .AppendKeyValue("DO_NOT_ESCAPE", _doNotEscape)
                        .AppendKeyValue("QUERY_BY_SETTLEMENT", _queryBySettlement)
                        .AppendKeyValue("QUERY_BY_HIERARCHY", _queryByHierarchy)
                        .AppendKeyValue("TPS_HASH_TYPE", _tpsHashType)
                        .AppendKeyValue("EXCLUDE_ERRORS", _excludeErrors);
                    break;
                case "stq":
                    CalcReportTPS();
                    _url = new Uri("https://secure.bluepay.com/interfaces/stq", UriKind.Absolute);
                    postData.AppendKeyValue("ACCOUNT_ID", _accountID)
                        .AppendKeyValue("MODE", _mode)
                        .AppendKeyValue("TAMPER_PROOF_SEAL", _tps)
                        .AppendKeyValue("REPORT_START_DATE", _reportStartDate)
                        .AppendKeyValue("REPORT_END_DATE", _reportEndDate)
                        .AppendKeyValue("TPS_HASH_TYPE", _tpsHashType)
                        .AppendKeyValue("EXCLUDE_ERRORS", _excludeErrors)
                        .AppendKeyValue("id", _masterID)
                        .AppendKeyValue("payment_type", _paymentType)
                        .AppendKeyValue("trans_type", _transType)
                        .AppendKeyValue("amount", _amount)
                        .AppendKeyValue("name1", _firstName)
                        .AppendKeyValue("name2", _lastName);
                    break;
                case "bp10emu":
                    CalcTPS();
                    _url = new Uri("https://secure.bluepay.com/interfaces/bp10emu", UriKind.Absolute);
                    postData.AppendKeyValue("MERCHANT", _accountID)
                        .AppendKeyValue("MODE", _mode)
                        .AppendKeyValue("TRANSACTION_TYPE", _transType)
                        .AppendKeyValue("TAMPER_PROOF_SEAL", _tps)
                        .AppendKeyValue("NAME1", _firstName)
                        .AppendKeyValue("NAME2", _lastName)
                        .AppendKeyValue("COMPANY_NAME", _companyName)
                        .AppendKeyValue("AMOUNT", _amount)
                        .AppendKeyValue("ADDR1", _addr1)
                        .AppendKeyValue("ADDR2", _addr2)
                        .AppendKeyValue("CITY", _city)
                        .AppendKeyValue("STATE", _state)
                        .AppendKeyValue("COUNTRY", _country)
                        .AppendKeyValue("ZIPCODE", _zip)
                        .AppendKeyValue("COMMENT", _memo)
                        .AppendKeyValue("PHONE", _phone)
                        .AppendKeyValue("EMAIL", _email)
                        .AppendKeyValue("REBILLING", _doRebill)
                        .AppendKeyValue("REB_FIRST_DATE", _rebillFirstDate)
                        .AppendKeyValue("REB_EXPR", _rebillExpr)
                        .AppendKeyValue("REB_CYCLES", _rebillCycles)
                        .AppendKeyValue("REB_AMOUNT", _rebillAmount)
                        .AppendKeyValue("RRNO", _masterID)
                        .AppendKeyValue("PAYMENT_TYPE", _paymentType)
                        .AppendKeyValue("INVOICE_ID", _invoiceID)
                        .AppendKeyValue("ORDER_ID", _orderID)
                        .AppendKeyValue("CUSTOM_ID", _customID1)
                        .AppendKeyValue("CUSTOM_ID2", _customID2)
                        .AppendKeyValue("AMOUNT_TIP", _amountTip)
                        .AppendKeyValue("AMOUNT_TAX", _amountTax)
                        .AppendKeyValue("AMOUNT_FOOD", _amountFood)
                        .AppendKeyValue("AMOUNT_MISC", _amountMisc)
                        .AppendKeyValue("CUSTOMER_IP", _environment.PublicIpAddress)
                        .AppendKeyValue("TPS_HASH_TYPE", _tpsHashType)
                        .AppendKeyValue("RESPONSEVERSION", 5);
                    if (_swipeData.HasValue())
                    {
                        var matchTrack1And2 = _track1And2.Match(_swipeData);
                        var matchTrack2 = _track2Only.Match(_swipeData);
                        if (matchTrack1And2.Success || matchTrack2.Success)
                        {
                            postData.AppendKeyValue("SWIPE", _swipeData);
                        }
                    }
                    else if (_paymentType == "CREDIT")
                    {
                        postData.AppendKeyValue("CC_NUM", _paymentAccount)
                            .AppendKeyValue("CC_EXPIRES", _cardExpire)
                            .AppendKeyValue("CVCCVV2", _cvv2);
                    }
                    else
                    {
                        postData.AppendKeyValue("ACH_ROUTING", _routingNum)
                            .AppendKeyValue("ACH_ACCOUNT", _accountNum)
                            .AppendKeyValue("ACH_ACCOUNT_TYPE", _accountType)
                            .AppendKeyValue("DOC_TYPE", _docType);
                    }
                    break;
                case "bp20rebadmin":
                    CalcRebillTPS();
                    _url = new Uri("https://secure.bluepay.com/interfaces/bp20rebadmin", UriKind.Absolute);
                    postData.AppendKeyValue("ACCOUNT_ID", _accountID)
                        .AppendKeyValue("TAMPER_PROOF_SEAL", _tps)
                        .AppendKeyValue("TRANS_TYPE", _transType)
                        .AppendKeyValue("REBILL_ID", _rebillID)
                        .AppendKeyValue("TEMPLATE_ID", _templateID)
                        .AppendKeyValue("REB_EXPR", _rebillExpr)
                        .AppendKeyValue("REB_CYCLES", _rebillCycles)
                        .AppendKeyValue("REB_AMOUNT", _rebillAmount)
                        .AppendKeyValue("NEXT_AMOUNT", _rebillNextAmount)
                        .AppendKeyValue("NEXT_DATE", _rebillNextDate)
                        .AppendKeyValue("TPS_HASH_TYPE", _tpsHashType)
                        .AppendKeyValue("STATUS", _rebillStatus);
                    break;
                default:
                    throw new InvalidOperationException(Res.BluePayInvalidApi);
            }

            // Add Level 2 data, if available.
            foreach (var field in _level2Info)
            {
                postData.AppendKeyValue(field.Key, field.Value);
            }

            // Add Level 3 item data, if available.
            foreach (var item in _lineItems)
            {
                foreach (var field in item)
                {
                    postData.AppendKeyValue(field.Key, field.Value);
                }
            }

            // Add customer token data, if available.
            postData.AppendKeyValue("NEW_CUST_TOKEN", _newCustToken);
            postData.AppendKeyValue("CUST_TOKEN", _custToken);

            return await PerformPostAsync(postData.ToString()).ConfigureAwait(false);
        }

        private async Task<BluePayResponse> PerformPostAsync(string post)
        {
            post ??= "";
            using var content = new StringContent(post, Encoding.ASCII, "application/x-www-form-urlencoded");
            content.Headers.ContentLength = post.Length;

            var httpResponse = await _httpClient.PostAsync(_url, content).ConfigureAwait(false);

            var response = string.Empty;
            if (_api == "bp10emu")
            {
                response = httpResponse.Headers.Location.ToString();
            }
            else
            {
                response = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            // Parse response.
            return new BluePayResponse(response);
        }
    }
}
