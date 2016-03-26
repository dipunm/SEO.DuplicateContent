using System;
using System.Linq;
using ReturnNull.CanonicalRoutes.Helpers;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class OrderQuerystrings : ICanonicalRule {
        public bool HasBeenViolated(Uri url, RouteInfo routeInfo, UserProvisions provisions)
        {
            var queryKeys = url.Query.ToKeyValuePairs()
                  .TakeWhile(pair => provisions.CanonicalQuerystrings.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase))
                  .Select(p => p.Key);
            var orderedQueryKeys = url.Query.ToKeyValuePairs()
                .Where(pair => provisions.CanonicalQuerystrings.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase))
                .OrderBy(pair => pair.Key)
                .Select(pair => pair.Key);
            return !queryKeys.SequenceEqual(orderedQueryKeys);
        }

        public void CorrectPlan(UrlPlan plan, RouteInfo routeInfo, UserProvisions provisions)
        {
            var excessPairs = plan.Query.ToKeyValuePairs()
                .Where(pair => !provisions.CanonicalQuerystrings.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase));

            var orderedCanonicalPairs = plan.Query.ToKeyValuePairs()
                .Where(pair => provisions.CanonicalQuerystrings.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase))
                .OrderBy(pair => pair.Key);

            var queries = new[]
            {
                orderedCanonicalPairs.ToQuerystring(),
                excessPairs.ToQuerystring()
            };

            plan.Query = string.Join("&", queries.Where(q => !string.IsNullOrWhiteSpace(q)));
        }
    }
}