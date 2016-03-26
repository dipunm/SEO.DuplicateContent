using System;
using System.Collections.Generic;
using System.Linq;
using ReturnNull.CanonicalRoutes.Helpers;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class LowercaseQuerystringValues : ICanonicalRule {
        public bool HasBeenViolated(RequestData requestData, UserProvisions provisions)
        {
            return requestData.RequestUri.Query.ToKeyValuePairs()
                  .Where(pair => provisions.CanonicalQuerystrings.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase))
                  .Where(pair => !provisions.SensitiveParameters.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase))
                  .Any(pair => !string.IsNullOrEmpty(pair.Value) && pair.Value.Any(char.IsUpper));
        }

        public void CorrectPlan(UrlPlan plan, RequestData requestData, UserProvisions provisions)
        {
            var correctedPairs = plan.Query.ToKeyValuePairs()
                .Select(pair =>
                {
                    if (provisions.CanonicalQuerystrings.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase)
                        && !provisions.SensitiveParameters.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase))
                        return new KeyValuePair<string, string>(pair.Key, pair.Value?.ToLowerInvariant());
                    return pair;
                });
            plan.Query = correctedPairs.ToQuerystring();
        }
    }
}