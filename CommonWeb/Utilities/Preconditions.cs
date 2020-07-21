using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Res = HanumanInstitute.CommonWeb.Properties.Resources;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Provides helper methods to validate parameters.
    /// </summary>
    public static class Preconditions
    {
        /// <summary>
        /// Validates whether specified value is not null, and throws an exception if it is null.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="name">The name of the parameter.</param>
        public static T CheckNotNull<T>(this T value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
            return value;
        }

        /// <summary>
        /// Validates whether specified value is not null or empty, and throws an exception if it is null or empty.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="name">The name of the parameter.</param>
        public static string CheckNotNullOrEmpty(this string? value, string name)
        {
            value.CheckNotNull(name);
            if (string.IsNullOrEmpty(value))
            {
                ThrowArgumentEmpty(name);
            }
            return value!;
        }

        /// <summary>
        /// Validates whether specified list is not null or empty, and throws an exception if it is null or empty.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="name">The name of the parameter.</param>
        public static IEnumerable<T> CheckNotNullOrEmpty<T>(this IEnumerable<T>? value, string name)
        {
            value.CheckNotNull(name);
            if (!value.Any())
            {
                ThrowArgumentEmpty(name);
            }
            return value!;
        }

        /// <summary>
        /// Returns whether specified value is in valid range.
        /// </summary>
        /// <typeparam name="T">The type of data to validate.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="min">The minimum valid value.</param>
        /// <param name="minInclusive">Whether the minimum value is valid.</param>
        /// <param name="max">The maximum valid value.</param>
        /// <param name="maxInclusive">Whether the maximum value is valid.</param>
        /// <returns>Whether the value is within range.</returns>
        public static bool IsInRange<T>(this T value, T? min = null, bool minInclusive = true, T? max = null, bool maxInclusive = true)
            where T : struct, IComparable<T>
        {
            var minValid = min == null || (minInclusive && value.CompareTo(min.Value) >= 0) || (!minInclusive && value.CompareTo(min.Value) > 0);
            var maxValid = max == null || (maxInclusive && value.CompareTo(max.Value) <= 0) || (!maxInclusive && value.CompareTo(max.Value) < 0);
            return minValid && maxValid;
        }

        /// <summary>
        /// Validates whether specified value is in valid range, and throws an exception if out of range.
        /// </summary>
        /// <typeparam name="T">The type of data to validate.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="min">The minimum valid value.</param>
        /// <param name="minInclusive">Whether the minimum value is valid.</param>
        /// <param name="max">The maximum valid value.</param>
        /// <param name="maxInclusive">Whether the maximum value is valid.</param>
        /// <returns>The value if valid.</returns>
        public static T CheckRange<T>(this T value, string name, T? min = null, bool minInclusive = true, T? max = null, bool maxInclusive = true)
            where T : struct, IComparable<T>
        {
            if (!value.IsInRange(min, minInclusive, max, maxInclusive))
            {
                if (min.HasValue && minInclusive && max.HasValue && maxInclusive)
                {
                    var message = Res.ValueRangeBetween;
                    throw new ArgumentOutOfRangeException(name, value, message.FormatInvariant(name, min, max));
                }
                else
                {
                    var messageMin = min.HasValue ? GetOpText(true, minInclusive).FormatInvariant(min) : null;
                    var messageMax = max.HasValue ? GetOpText(false, maxInclusive).FormatInvariant(max) : null;
                    var message = (messageMin != null && messageMax != null) ?
                        Res.ValueRangeAnd :
                        Res.ValueRange;
                    throw new ArgumentOutOfRangeException(name, value, message.FormatInvariant(name, messageMin ?? messageMax, messageMax));
                }
            }
            return value;
        }

        private static string GetOpText(bool greaterThan, bool inclusive)
        {
            return (greaterThan && inclusive) ? Res.ValueRangeGreaterThanInclusive :
                greaterThan ? Res.ValueRangeGreaterThan :
                inclusive ? Res.ValueRangeLessThanInclusive :
                Res.ValueRangeLessThan;
        }

        /// <summary>
        /// Validates whether specified type is assignable from specific base class.
        /// </summary>
        /// <param name="value">The Type to validate.</param>
        /// <param name="baseType">The base type that value type must derive from.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns></returns>
        //public static Type CheckAssignableFrom(this Type value, Type baseType, string name)
        //{
        //    value.CheckNotNull(name);
        //    baseType.CheckNotNull(nameof(baseType));

        //    if (!value.IsAssignableFrom(baseType))
        //    {
        //        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.TypeMustBeAssignableFromBase, name, value.Name, baseType.Name));
        //    }
        //    return value;
        //}

        /// <summary>
        /// Validates whether specified type derives from specific base class.
        /// </summary>
        /// <param name="value">The Type to validate.</param>
        /// <param name="baseType">The base type that value type must derive from.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns></returns>
        //public static Type CheckDerivesFrom(this Type value, Type baseType, string name)
        //{
        //    value.CheckNotNull(name);
        //    baseType.CheckNotNull(nameof(baseType));

        //    if (!value.IsSubclassOf(baseType))
        //    {
        //        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.TypeMustDeriveFromBase, name, value.Name, baseType.Name));
        //    }
        //    return value;
        //}

        /// <summary>
        /// Throws an exception of type ArgumentException saying an argument is null or empty.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        private static void ThrowArgumentEmpty(string name)
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Res.ValueEmpty, name), name);
        }
    }
}
