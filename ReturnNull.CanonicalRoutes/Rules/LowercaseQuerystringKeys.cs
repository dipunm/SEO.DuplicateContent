using System;
using System.Collections.Generic;
using System.Linq;
using ReturnNull.CanonicalRoutes.Helpers;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class LowercaseQuerystringKeys : ICanonicalRule {
        public bool HasBeenViolated(RequestData requestData, UserProvisions provisions)
        {
            return requestData.RequestUri.Query.ToKeyValuePairs()
                .Where(pair => provisions.CanonicalQuerystrings.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase))
                .Any(pair => !string.IsNullOrEmpty(pair.Key) && pair.Key.Any(char.IsUpper));
        }

        public void CorrectPlan(UrlPlan plan, RequestData requestData, UserProvisions provisions)
        {
            var correctedPairs = plan.Query.ToKeyValuePairs()
                .Select(pair =>
                {
                    if (provisions.CanonicalQuerystrings.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase))
                        return new KeyValuePair<string, string>(pair.Key?.ToLowerInvariant(), pair.Value);
                    return pair;
                });
            plan.Query = correctedPairs.ToQuerystring();
        }
    }
}