using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace HanumanInstitute.CommonWeb
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts a value to string using InvariantCulture.
        /// </summary>
        /// <typeparam name="T">The type of object to convert.</typeparam>
        /// <param name="value">The object to convert to string.</param>
        /// <returns>The invariant string representation of the object.</returns>
        public static string ToStringInvariant<T>(this T value) => FormattableString.Invariant($"{value}");

        /// <summary>
        /// Returns whether the string contains a value. It is the equivalent of !string.IsNullOrEmpty(value).
        /// </summary>
        /// <param name="value">The string to evaluate.</param>
        /// <returns>Whether the string is not null or empty.</returns>
        public static bool HasValue([NotNullWhen(true)] this string? value) => !string.IsNullOrEmpty(value);

        /// <summary>
        /// Returns specified default value if the value is null or empty.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <param name="defaultValue">The default value if value is null or empty.</param>
        /// <returns>The new value.</returns>
        public static string Default(this string value, string defaultValue) => string.IsNullOrEmpty(value) ? defaultValue : value;

        /// <summary>
        /// Formats a string using invariant culture. This is a shortcut for string.format(CultureInfo.InvariantCulture, ...)
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatInvariant(this string format, params object?[] args) => string.Format(CultureInfo.InvariantCulture, format, args);

        /// <summary>
        /// Appends specified value to the StringBuilder only if cond is true.
        /// </summary>
        /// <param name="builder">The StringBuilder to append to.</param>
        /// <param name="cond">Whether to add value to the StringBuilder.</param>
        /// <param name="value">The value to append.</param>
        /// <returns>The StringBuilder.</returns>
        public static StringBuilder AppendIf(this StringBuilder builder, bool cond, string value)
        {
            builder.CheckNotNull(nameof(builder));
            if (cond)
            {
                builder.Append(value);
            }
            return builder;
        }

        /// <summary>
        /// Appends a value affter calling HttpUtility.UrlEncode
        /// </summary>
        /// <param name="builder">The StringBuilder to append to.</param>
        /// <param name="value">The value to append.</param>
        /// <returns>The StringBuilder.</returns>
        public static StringBuilder AppendUrlEncode(this StringBuilder builder, string? value)
        {
            builder.CheckNotNull(nameof(builder));
            if (value.HasValue())
            {
                builder.Append(HttpUtility.UrlEncode(value));
            }
            return builder;
        }

        /// <summary>
        /// Converts a dictionary to a URI-encoded string.
        /// </summary>
        /// <param name="parameters">The parameters to encode.</param>
        /// <returns>A URI-encoded string.</returns>
        public static string ToQueryString(this IDictionary<string, object> parameters) =>
            string.Join("&", parameters.Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value.ToString())}"));

        /// <summary>
        /// Processes an enumeration in parallel in an awaitable way.
        /// </summary>
        /// <typeparam name="TSource">The type of the source list.</typeparam>
        /// <typeparam name="TResult">The result of the operation run on each item.</typeparam>
        /// <param name="source">The source list to iterate.</param>
        /// <param name="taskSelector">The operation to evaluate for each item.</param>
        /// <param name="resultProcessor">Callback after each item is evaluated.</param>
        public static Task ForEachAsync<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Task<TResult>> taskSelector, Action<TSource, TResult> resultProcessor)
        {
            using (var oneAtATime = new SemaphoreSlim(10, 10))
            {
                return Task.WhenAll(
                    from item in source
                    select ProcessAsync(item, taskSelector, resultProcessor, oneAtATime));
            }

            static async Task ProcessAsync(
                TSource item,
                Func<TSource, Task<TResult>> taskSelector, Action<TSource, TResult> resultProcessor,
                SemaphoreSlim oneAtATime)
            {
                await oneAtATime.WaitAsync().ConfigureAwait(false);
                var result = await taskSelector(item).ConfigureAwait(false);
                try
                {
                    resultProcessor(item, result);
                }
                finally
                {
                    oneAtATime.Release();
                }
            }
        }

        /// <summary>
        /// Processes a list in parallel in an awaitable way and returns the output in the same order.
        /// </summary>
        /// <typeparam name="TSource">The type of the source list.</typeparam>
        /// <typeparam name="TResult">The result of the operation run on each item.</typeparam>
        /// <param name="source">The source list to iterate.</param>
        /// <param name="taskSelector">The operation to evaluate for each item.</param>
        /// <returns>The list of results in the same order as source.</returns>
        public static async Task<IList<TResult>> ForEachOrderedAsync<TSource, TResult>(
            this IList<TSource> source,
            Func<TSource, Task<TResult>> taskSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var oneAtATime = new SemaphoreSlim(10, 10))
            {
                var indexList = new TResult[source.Count];
                var taskList = new Task[source.Count];
                for (var i = 0; i < source.Count; i++)
                {
                    taskList[i] = ProcessOrderedAsync(source[i], taskSelector, oneAtATime, i, (src, res, index) =>
                    {
                        indexList[index] = res;
                    });
                }
                await Task.WhenAll(taskList).ConfigureAwait(false);

                var result = new List<TResult>(source.Count);
                for (var i = 0; i < source.Count; i++)
                {
                    result.Add(indexList[i]);
                }
                return result;
            }

            static async Task ProcessOrderedAsync(
                TSource item,
                Func<TSource, Task<TResult>> taskSelector,
                SemaphoreSlim oneAtATime, int index, Action<TSource, TResult, int> resultProcessor)
            {
                await oneAtATime.WaitAsync().ConfigureAwait(false);
                var result = await taskSelector(item).ConfigureAwait(false);
                try
                {
                    resultProcessor(item, result, index);
                }
                finally
                {
                    oneAtATime.Release();
                }
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <typeparam name="T">The type of data contained in the list.</typeparam>
        /// <param name="list">The list to append to.</param>
        /// <param name="items">The items to append.</param>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (list is List<T> asList)
            {
                asList.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }
    }
}
