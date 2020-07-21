using System;
using System.Collections.Generic;

namespace HanumanInstitute.WebStore.Models
{
    /// <summary>
    /// Contains information about products configured in Ontraport.
    /// </summary>
    public class ProductNames
    {
        private IDictionary<string, string> _products = new Dictionary<string, string>()
        {
            // Product display names.
            { AlchemyRings, "Alchemy Rings" },
            { AuthenticAttraction, "Authentic Attraction"},
            { CrystalAttunement, "Crystal Attunement"},
            { Deposit, "Deposit"},
            { Donation, "Donation"},
            { EmergenceGuardianContinuationProgram, "Emergence Guardian Continuation Program"},
            { GodConnectionAttunement, "God Connection Attunement"},
            { GodMoneyMasterclass, "God & Money Masterclass"},
            { LiveEvent, "Live Event"},
            { PowerliminalsNonRivalry, "Non-Rivalry Powerliminals"},
            { PrivateCoaching, "1-on-1 Coaching"},
            { ResultOrientedSpirituality, "Result-Oriented Spirituality"},
            { SexualMagnetism, "Sexual Magnetism"},
            { SoulAlignmentReading, "Energy Tune Up" }
        };

        /// <summary>
        /// Returns the display name of specified product.
        /// </summary>
        /// <param name="product">The product to retrieve information for.</param>
        /// <returns>The product information.</returns>
        /// <exception cref="KeyNotFoundException">Product name is not found in the dictionary.</exception>
        public string GetDisplayName(string name) => _products.ContainsKey(name) ? _products[name] : name + " (invalid product code)";

        // Product codes configured in Ontraport.
        public const string AlchemyRings = "Alchemy Rings";
        public const string AuthenticAttraction = "Authentic Attraction";
        public const string CrystalAttunement = "Crystal Activation";
        public const string Deposit = "Deposit";
        public const string Donation = "Donation";
        public const string EmergenceGuardianContinuationProgram = "Emergence Guardian Continuation Program";
        public const string GodConnectionAttunement = "God Connection Attunement";
        public const string GodMoneyMasterclass = "God Money Masterclass";
        public const string LiveEvent = "Live Event";
        public const string PowerliminalsNonRivalry = "Non-Rivalry Powerliminals";
        public const string PrivateCoaching = "1-on-1 Coaching";
        public const string ResultOrientedSpirituality = "Result-Oriented Spirituality";
        public const string SexualMagnetism = "Sexual Magnetism";
        public const string SoulAlignmentReading = "Energy Tune Up";
    }
}
