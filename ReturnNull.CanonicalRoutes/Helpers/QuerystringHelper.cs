using System.Collections.Generic;
using System.Linq;

namespace ReturnNull.CanonicalRoutes.Helpers
{
    public static class QuerystringHelper
    {
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(this string querystring, char pairSeparator = '&', char keyValueSeparator = '=')
        {
            return querystring.TrimStart('?').Split(pairSeparator)
                .Where(param => !string.IsNullOrWhiteSpace(param))
                .Select(param => param.Split(new[] {keyValueSeparator}, 2))
                .Select(split => new KeyValuePair<string, string>(split.First(), split.ElementAtOrDefault(1)));
        }

        public static string ToQuerystring(this IEnumerable<KeyValuePair<string, string>> pairs)
        {
            return string.Join("&", pairs.Select(pair => 
                string.IsNullOrEmpty(pair.Value) ? 
                    pair.Key : 
                    $"{pair.Key}={pair.Value}"));
        }
    }
}