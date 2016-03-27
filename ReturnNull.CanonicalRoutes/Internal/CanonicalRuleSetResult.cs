using System;
using ReturnNull.CanonicalRoutes.Models;

namespace ReturnNull.CanonicalRoutes.Internal
{
    internal class CanonicalRulesetResult
    {
        public CanonicalRulesetResult(bool shouldRedirect, UrlPlan plan)
        {
            ShouldRedirect = shouldRedirect;
            Plan = plan;
        }

        public bool ShouldRedirect { get; }
        public UrlPlan Plan { get; }
    }
}