using System;
using System.Linq;
using ReturnNull.CanonicalRoutes.Helpers;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class RemoveUncanonicalQuerystrings : ICanonicalRule {
        public bool HasBeenViolated(Uri url, RouteInfo routeInfo, UserProvisions provisions)
        {
            return url.Query.ToKeyValuePairs()
                .Any(pair => !provisions.CanonicalQuerystrings.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase));
        }

        public void CorrectPlan(UrlPlan plan, RouteInfo routeInfo, UserProvisions provisions)
        {
            var correctedPairs = plan.Query.ToKeyValuePairs()
                   .Where(pair => provisions.CanonicalQuerystrings.Contains(pair.Key, StringComparer.InvariantCultureIgnoreCase));
            plan.Query = correctedPairs.ToQuerystring();
        }
    }
}