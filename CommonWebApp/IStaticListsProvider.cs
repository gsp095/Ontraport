using System;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Provides options lists related to payments.
    /// </summary>
    public interface IStaticListsProvider
    {
        /// <summary>
        /// Gets the list of expiration months to choose from.
        /// </summary>
        ListKeyValue<int?> ExpirationMonths { get; }
        /// <summary>
        /// Gets the list of expiration years to choose from.
        /// </summary>
        ListKeyValue<int?> ExpirationYears { get; }
        /// <summary>
        /// Returns the list of all countries to choose from.
        /// </summary>
        ListKeyValue Countries { get; }
        /// <summary>
        /// Returns the list of all states to choose from.
        /// </summary>
        ListKeyValue States { get; }
    }
}
