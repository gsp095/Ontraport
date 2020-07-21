using System;
using HanumanInstitute.CommonWeb.Payments;
using HanumanInstitute.OntraportApi.Converters;
using HanumanInstitute.OntraportApi.Models;

namespace HanumanInstitute.CommonWeb.Ontraport
{
    /// <summary>
    /// Contact objects allow you to keep up-to-date records for all the contacts you are managing.
    /// </summary>
    public class ApiCustomContact : ApiContact
    {
        /// <summary>
        /// Returns a ApiProperty object to get or set the contact's age.
        /// </summary>
        public ApiProperty<int> AgeField => _ageField ??= new ApiProperty<int>(this, AgeKey);
        private ApiProperty<int>? _ageField;
        public const string AgeKey = "f1336";
        /// <summary>
        /// Gets or sets the contact's age.
        /// </summary>
        public int? Age { get => AgeField.Value; set => AgeField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the gender of the contact.
        /// </summary>
        public ApiPropertyStringEnum<ContactGender> GenderField => _genderField ??= new ApiPropertyStringEnum<ContactGender>(this, GenderKey);
        private ApiPropertyStringEnum<ContactGender>? _genderField;
        public const string GenderKey = "f1337";
        /// <summary>
        /// Gets or sets the gender of the contact.
        /// </summary>
        public ContactGender? Gender { get => GenderField.Value; set => GenderField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the name of the client's picture file within the pictures folder.
        /// </summary>
        public ApiPropertyString PictureFileNameField => _pictureFileNameField ??= new ApiPropertyString(this, PictureFileNameKey);
        private ApiPropertyString? _pictureFileNameField;
        public const string PictureFileNameKey = "f1338";
        /// <summary>
        /// Gets or sets the name of the name of the client's picture file within the pictures folder.
        /// </summary>
        public string? PictureFileName { get => PictureFileNameField.Value; set => PictureFileNameField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set how many coaching calls the client still has.
        /// </summary>
        public ApiProperty<int> CoachingCallsLeftField => _coachingCallsLeftField ??= new ApiProperty<int>(this, CoachingCallsLeftKey);
        private ApiProperty<int>? _coachingCallsLeftField;
        public const string CoachingCallsLeftKey = "f1342";
        /// <summary>
        /// Gets or sets how many coaching calls the client still has.
        /// </summary>
        public int? CoachingCallsLeft { get => CoachingCallsLeftField.Value; set => CoachingCallsLeftField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the date of the client's last coaching call.
        /// </summary>
        public ApiPropertyDateTime LastCoachingDateField => _lastCoachingDateField ??= new ApiPropertyDateTime(this, LastCoachingDateKey);
        private ApiPropertyDateTime? _lastCoachingDateField;
        public const string LastCoachingDateKey = "f1343";
        /// <summary>
        /// Gets or sets the date of the client's last coaching call.
        /// </summary>
        public DateTimeOffset? LastCoachingDate { get => LastCoachingDateField.Value; set => LastCoachingDateField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the answer to the question: how did you hear about us?
        /// </summary>
        public ApiPropertyString HearAboutUsField => _hearAboutUsField ??= new ApiPropertyString(this, HearAboutUsKey);
        private ApiPropertyString? _hearAboutUsField;
        public const string HearAboutUsKey = "f1360";
        /// <summary>
        /// Gets or sets the answer to the question: how did you hear about us?
        /// </summary>
        public string? HearAboutUs { get => HearAboutUsField.Value; set => HearAboutUsField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the strategy session answer to the question: What do you truly want to accomplish? these issues?
        /// </summary>
        public ApiPropertyString StrategyGoalsField => _strategyGoalsField ??= new ApiPropertyString(this, StrategyGoalsKey);
        private ApiPropertyString? _strategyGoalsField;
        public const string StrategyGoalsKey = "f1370";
        /// <summary>
        /// Gets or sets the strategy session answer to the question: What do you truly want to accomplish?
        /// </summary>
        public string? StrategyGoals { get => StrategyGoalsField.Value; set => StrategyGoalsField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the strategy session answer to the question: What issues are you currently experiencing?
        /// </summary>
        public ApiPropertyString StrategyIssuesField => _strategyIssuesField ??= new ApiPropertyString(this, StrategyIssuesKey);
        private ApiPropertyString? _strategyIssuesField;
        public const string StrategyIssuesKey = "f1371";
        /// <summary>
        /// Gets or sets the strategy session answer to the question: What issues are you currently experiencing?
        /// </summary>
        public string? StrategyIssues { get => StrategyIssuesField.Value; set => StrategyIssuesField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the strategy session answer to the question: What steps have you taken to overcome these issues?
        /// </summary>
        public ApiPropertyString StrategyStepsField => _strategyStepsField ??= new ApiPropertyString(this, StrategyStepsKey);
        private ApiPropertyString? _strategyStepsField;
        public const string StrategyStepsKey = "f1372";
        /// <summary>
        /// Gets or sets the strategy session answer to the question: What steps have you taken to overcome these issues?
        /// </summary>
        public string? StrategySteps { get => StrategyStepsField.Value; set => StrategyStepsField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the strategy session answer to the question: Why is NOW your time to transform?
        /// </summary>
        public ApiPropertyString StrategyWhyField => _strategyWhyField ??= new ApiPropertyString(this, StrategyWhyKey);
        private ApiPropertyString? _strategyWhyField;
        public const string StrategyWhyKey = "f1373";
        /// <summary>
        /// Gets or sets the strategy session answer to the question: Why is NOW your time to transform?
        /// </summary>
        public string? StrategyWhy { get => StrategyWhyField.Value; set => StrategyWhyField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the client's main goal after a strategy session.
        /// </summary>
        public ApiPropertyString MainGoalField => _mainGoalField ??= new ApiPropertyString(this, MainGoalKey);
        private ApiPropertyString? _mainGoalField;
        public const string MainGoalKey = "f1383";
        /// <summary>
        /// Gets or sets the client's main goal after a strategy session.
        /// </summary>
        public string? MainGoal { get => MainGoalField.Value; set => MainGoalField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the client's main obstacle after a strategy session.
        /// </summary>
        public ApiPropertyString MainObstacleField => _mainObstacleField ??= new ApiPropertyString(this, MainObstacleKey);
        private ApiPropertyString? _mainObstacleField;
        public const string MainObstacleKey = "f1384";
        /// <summary>
        /// Gets or sets the client's main obstacle after a strategy session.
        /// </summary>
        public string? MainObstacle { get => MainObstacleField.Value; set => MainObstacleField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the strategy session answer to the question: how would you rate the intensity of your pain?
        /// </summary>
        public ApiProperty<int> StrategyRatePainField => _strategyRatePainField ??= new ApiProperty<int>(this, StrategyRatePainKey);
        private ApiProperty<int>? _strategyRatePainField;
        public const string StrategyRatePainKey = "f1556";
        /// <summary>
        /// Gets or sets the strategy session answer to the question: how would you rate the intensity of your pain?
        /// </summary>
        public int? StrategyRatePain { get => StrategyRatePainField.Value; set => StrategyRatePainField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the strategy session answer to the question: your desire to change?
        /// </summary>
        public ApiProperty<int> StrategyRateDesireField => _strategyRateDesireField ??= new ApiProperty<int>(this, StrategyRateDesireKey);
        private ApiProperty<int>? _strategyRateDesireField;
        public const string StrategyRateDesireKey = "f1557";
        /// <summary>
        /// Gets or sets the strategy session answer to the question: how would you rate your desire to change?
        /// </summary>
        public int? StrategyRateDesire { get => StrategyRateDesireField.Value; set => StrategyRateDesireField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the strategy session answer to the question: how would you rate your urgency to change?
        /// </summary>
        public ApiProperty<int> StrategyRateUrgencyField => _strategyRateUrgencyField ??= new ApiProperty<int>(this, StrategyRateUrgencyKey);
        private ApiProperty<int>? _strategyRateUrgencyField;
        public const string StrategyRateUrgencyKey = "f1558";
        /// <summary>
        /// Gets or sets the strategy session answer to the question: how would you rate your urgency to change?
        /// </summary>
        public int? StrategyRateUrgency { get => StrategyRateUrgencyField.Value; set => StrategyRateUrgencyField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the strategy session answer to the question: What income do you aim to generate in the next 12 months?
        /// </summary>
        public ApiPropertyString StrategyIncomeGoalField => _strategyIncomeGoalField ??= new ApiPropertyString(this, StrategyIncomeGoalKey);
        private ApiPropertyString? _strategyIncomeGoalField;
        public const string StrategyIncomeGoalKey = "f1559";
        /// <summary>
        /// Gets or sets the strategy session answer to the question: What income do you aim to generate in the next 12 months?
        /// </summary>
        public string? StrategyIncomeGoal { get => StrategyIncomeGoalField.Value; set => StrategyIncomeGoalField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set notes when delivering the God Connection Attunement service.
        /// </summary>
        public ApiPropertyString GodConnectionNotesField => _godConnectionNotesField ??= new ApiPropertyString(this, GodConnectionNotesKey);
        private ApiPropertyString? _godConnectionNotesField;
        public const string GodConnectionNotesKey = "f1842";
        /// <summary>
        /// Gets or sets notes when delivering the God Connection Attunement service.
        /// </summary>
        public string? GodConnectionNotes { get => GodConnectionNotesField.Value; set => GodConnectionNotesField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the URL of the God & Money Worksheet.
        /// </summary>
        public ApiPropertyString GodMoneyWorksheetUrlField => _godMoneyWorksheetUrlField ??= new ApiPropertyString(this, GodMoneyWorksheetUrlKey);
        private ApiPropertyString? _godMoneyWorksheetUrlField;
        public const string GodMoneyWorksheetUrlKey = "f1877";
        /// <summary>
        /// Gets or sets the URL of the God & Money Worksheet.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Reviewed: return raw string data from Ontraport")]
        public string? GodMoneyWorksheetUrl { get => GodMoneyWorksheetUrlField.Value; set => GodMoneyWorksheetUrlField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the payment method for recurring payments.
        /// </summary>
        public ApiPropertyStringEnum<PaymentMethod> RecurringPaymentMethodField => _recurringPaymentMethodField ??= new ApiPropertyStringEnum<PaymentMethod>(this, RecurringPaymentMethodKey);
        private ApiPropertyStringEnum<PaymentMethod>? _recurringPaymentMethodField;
        public const string RecurringPaymentMethodKey = "f1919";
        /// <summary>
        /// Gets or sets the payment method for recurring payments.
        /// </summary>
        public PaymentMethod? RecurringPaymentMethod { get => RecurringPaymentMethodField.Value; set => RecurringPaymentMethodField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the payment currency for recurring payments.
        /// </summary>
        public ApiPropertyStringEnum<Currency> RecurringPaymentCurrencyField => _recurringPaymentCurrencyField ??= new ApiPropertyStringEnum<Currency>(this, RecurringPaymentCurrencyKey);
        private ApiPropertyStringEnum<Currency>? _recurringPaymentCurrencyField;
        public const string RecurringPaymentCurrencyKey = "f1920";
        /// <summary>
        /// Gets or sets the payment currency for recurring payments.
        /// </summary>
        public Currency? RecurringPaymentCurrency { get => RecurringPaymentCurrencyField.Value; set => RecurringPaymentCurrencyField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set the last successful transaction MasterID from which to do recurring payments.
        /// </summary>
        public ApiProperty<int> RecurringLastMasterIdField => _recurringLastMasterIdField ??= new ApiProperty<int>(this, RecurringLastMasterIdKey);
        private ApiProperty<int>? _recurringLastMasterIdField;
        public const string RecurringLastMasterIdKey = "f1903";
        /// <summary>
        /// Gets or sets the last successful transaction MasterID from which to do recurring payments.
        /// </summary>
        public int? RecurringLastMasterId { get => RecurringLastMasterIdField.Value; set => RecurringLastMasterIdField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set which payment plan is managing the collection sequence, or null if payments are not in collection.
        /// </summary>
        public ApiProperty<int> CollectionPlanIdField => _collectionPlanIdField ??= new ApiProperty<int>(this, CollectionPlanIdKey);
        private ApiProperty<int>? _collectionPlanIdField;
        public const string CollectionPlanIdKey = "f1903";
        /// <summary>
        /// Gets or sets which payment plan is managing the collection sequence, or null if payments are not in collection.
        /// </summary>
        public int? CollectionPlanId { get => CollectionPlanIdField.Value; set => CollectionPlanIdField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set which payment plan is managing the collection sequence, or null if payments are not in collection.
        /// </summary>
        public ApiProperty<decimal> AccountCreditsField => _accountCreditsField ??= new ApiProperty<decimal>(this, AccountCreditsKey);
        private ApiProperty<decimal>? _accountCreditsField;
        public const string AccountCreditsKey = "f1911";
        /// <summary>
        /// Gets or sets which payment plan is managing the collection sequence, or null if payments are not in collection.
        /// </summary>
        public decimal? AccountCredits { get => AccountCreditsField.Value; set => AccountCreditsField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set whether the contact has paid any money for Satrimono Healing Technologies.
        /// </summary>
        public ApiPropertyBool HasPaidHealingTechnologiesField => _hasPaidHealingTechnologiesField ??= new ApiPropertyBool(this, HasPaidHealingTechnologiesKey);
        private ApiPropertyBool? _hasPaidHealingTechnologiesField;
        public const string HasPaidHealingTechnologiesKey = "f1912";
        /// <summary>
        /// Gets or sets whether the contact has paid any money for Satrimono Healing Technologies.
        /// </summary>
        public bool? HasPaidHealingTechnologies { get => HasPaidHealingTechnologiesField.Value; set => HasPaidHealingTechnologiesField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set whether the contact has done the Remote Cell Harmonizer trial.
        /// </summary>
        public ApiPropertyBool TrialRCHField => _trialRCHField ??= new ApiPropertyBool(this, TrialRCHKey);
        private ApiPropertyBool? _trialRCHField;
        public const string TrialRCHKey = "f1913";
        /// <summary>
        /// Gets or sets whether the contact has done the Remote Cell Harmonizer trial.
        /// </summary>
        public bool? TrialRCH { get => TrialRCHField.Value; set => TrialRCHField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set whether the contact has done the EMF Neutralizer Device trial.
        /// </summary>
        public ApiPropertyBool TrialEmfDeviceField => _trialEmfDeviceField ??= new ApiPropertyBool(this, TrialEmfDeviceKey);
        private ApiPropertyBool? _trialEmfDeviceField;
        public const string TrialEmfDeviceKey = "f1914";
        /// <summary>
        /// Gets or sets whether the contact has done the EMF Neutralizer Device trial.
        /// </summary>
        public bool? TrialEmfDevice { get => TrialEmfDeviceField.Value; set => TrialEmfDeviceField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set whether the contact has done the EMF Neutralizer Skin trial.
        /// </summary>
        public ApiPropertyBool TrialEmfSkinField => _trialEmfSkinField ??= new ApiPropertyBool(this, TrialEmfSkinKey);
        private ApiPropertyBool? _trialEmfSkinField;
        public const string TrialEmfSkinKey = "f1915";
        /// <summary>
        /// Gets or sets whether the contact has done the Remote Cell Harmonizer trial.
        /// </summary>
        public bool? TrialEmfSkin { get => TrialEmfSkinField.Value; set => TrialEmfSkinField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set whether the contact has done the EMF Neutralizer Home trial.
        /// </summary>
        public ApiPropertyBool TrialEmfHomeField => _trialEmfHomeField ??= new ApiPropertyBool(this, TrialEmfHomeKey);
        private ApiPropertyBool? _trialEmfHomeField;
        public const string TrialEmfHomeKey = "f1916";
        /// <summary>
        /// Gets or sets whether the contact has done the EMF Neutralizer Home trial.
        /// </summary>
        public bool? TrialEmfHome { get => TrialEmfHomeField.Value; set => TrialEmfHomeField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set whether the contact has done the Morgellon Healer trial.
        /// </summary>
        public ApiPropertyBool TrialMorgellonHealerField => _trialMorgellonHealerField ??= new ApiPropertyBool(this, TrialMorgellonHealerKey);
        private ApiPropertyBool? _trialMorgellonHealerField;
        public const string TrialMorgellonHealerKey = "f1917";
        /// <summary>
        /// Gets or sets whether the contact has done the Morgellon Healer trial.
        /// </summary>
        public bool? TrialMorgellonHealer { get => TrialMorgellonHealerField.Value; set => TrialMorgellonHealerField.Value = value; }

        /// <summary>
        /// Returns a ApiProperty object to get or set whether the contact has done the Driving Assistant trial.
        /// </summary>
        public ApiPropertyBool TrialDrivingAssistantField => _trialDrivingAssistantField ??= new ApiPropertyBool(this, TrialDrivingAssistantKey);
        private ApiPropertyBool? _trialDrivingAssistantField;
        public const string TrialDrivingAssistantKey = "f1918";
        /// <summary>
        /// Gets or sets whether the contact has done the Driving Assistant trial.
        /// </summary>
        public bool? TrialDrivingAssistant { get => TrialDrivingAssistantField.Value; set => TrialDrivingAssistantField.Value = value; }


        public enum ContactGender
        {
            Woman = 1,
            Man = 2
        }
    }
}
