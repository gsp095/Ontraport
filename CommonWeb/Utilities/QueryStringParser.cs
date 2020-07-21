using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

namespace HanumanInstitute.CommonWeb.Utilities
{
    public class QueryStringParser
    {
        private readonly string _queryString;

        public QueryStringParser(string? queryString)
        {
            _queryString = queryString ?? string.Empty;
        }

        public NameValueCollection Params => _params ??= HttpUtility.ParseQueryString(_queryString);
        private NameValueCollection? _params;


        public delegate bool TryParseHandler<T>(string value, NumberStyles style, IFormatProvider provider, out T result);

        private T? Parse<T>(string key, T? min, T? max, TryParseHandler<T> parseMethod, NumberStyles style)
            where T : struct, IComparable
        {
            var strValue = Params[key];
            if (!string.IsNullOrEmpty(strValue))
            {
                if (parseMethod(strValue, style, CultureInfo.InvariantCulture, out var value))
                {
                    if ((min == null || value.CompareTo(min) >= 0) && (max == null || value.CompareTo(max) <= 0))
                    {
                        return value;
                    }
                }
            }
            return null;
        }

        public int? GetInt32(string key, int? min = null, int? max = null) =>
            Parse(key, min, max, int.TryParse, NumberStyles.Integer);

        public long? GetInt64(string key, long? min = null, long? max = null) =>
            Parse(key, min, max, long.TryParse, NumberStyles.Integer);

        public float? GetSingle(string key, float? min = null, float? max = null) =>
            Parse(key, min, max, float.TryParse, NumberStyles.Float);

        public double? GetDouble(string key, double? min = null, double? max = null) =>
            Parse(key, min, max, double.TryParse, NumberStyles.Float);

        public decimal? GetDecimal(string key, decimal? min = null, decimal? max = null) =>
            Parse(key, min, max, decimal.TryParse, NumberStyles.AllowDecimalPoint);

        public bool? GetBool(string key)
        {
            var strValue = Params[key];
            if (!string.IsNullOrEmpty(strValue))
            {
                return string.Compare(strValue, "true", StringComparison.InvariantCultureIgnoreCase) == 0 || string.Compare(strValue, "1", StringComparison.InvariantCultureIgnoreCase) == 0;
            }
            return null;
        }
    }
}
