using System.Collections.Generic;
using System.Collections.Specialized;

namespace HanumanInstitute.CommonWeb
{
    /// <summary>
    /// A list of string KeyValuePair that can be initialized with a short { "", "" } syntax.
    /// </summary>
    public class ListKeyValue : ListKeyValue<string>
    { }

    /// <summary>
    /// A list of string KeyValuePair that can be initialized with a short { "", "" } syntax.
    /// </summary>
    public class ListKeyValue<T> : List<KeyValuePair<T, string>>
    {
        public void Add(T key, object value)
        {
            Add(new KeyValuePair<T, string>(key, value?.ToStringInvariant() ?? string.Empty));
        }

        public NameValueCollection AsNameValueCollection()
        {
            var result = new NameValueCollection();
            foreach (var item in this)
            {
                result.Add(item.Key.ToStringInvariant(), item.Value);
            }
            return result;
        }
    }
}
