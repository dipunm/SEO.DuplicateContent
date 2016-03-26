using System;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class RemoveRepeatingSlashes : CanonicalRuleSnitch {
        public override bool HasBeenViolated(Uri url, RouteInfo routeInfo, UserProvisions provisions)
        {
            return url.AbsolutePath.Contains("//");
        }
    }
}