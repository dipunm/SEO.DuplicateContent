using System;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class RemoveTrailingSlash : CanonicalRuleSnitch {
        public override bool HasBeenViolated(RequestData requestData, UserProvisions provisions)
        {
            return requestData.RequestUri.AbsolutePath != "/" && requestData.RequestUri.AbsolutePath.EndsWith("/");
        }
    }
}