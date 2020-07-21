using System.Text;
using System.Web;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// Utility class to help build URL query strings.
    /// </summary>
    public class QueryStringBuilder
    {
        private readonly StringBuilder _builder;

        public QueryStringBuilder()
        {
            _builder = new StringBuilder();
        }

        public QueryStringBuilder(string? value)
        {
            _builder = new StringBuilder(value);
        }

        /// <summary>
        /// Appends specified key-value pair to the query string, if value is not null or empty.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add. It will be converted to string using InvariantCulture and then Uri encoded.</param>
        /// <returns>The QueryStringBuilder.</returns>
        public QueryStringBuilder AppendKeyValue(string key, object? value) => AppendKeyValue(key, value?.ToStringInvariant());

        /// <summary>
        /// Appends specified key-value pair to the query string, if value is not null or empty.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add. It will be Uri encoded.</param>
        /// <returns>The QueryStringBuilder.</returns>
        public QueryStringBuilder AppendKeyValue(string key, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (_builder.Length > 0)
                {
                    _builder.Append("&");
                }
                _builder.Append(key);
                _builder.Append("=");
                _builder.Append(HttpUtility.UrlEncode(value));
            }
            return this;
        }

        /// <summary>
        /// Returns the full query string.
        /// </summary>
        public override string ToString() => _builder.ToString();
    }
}
