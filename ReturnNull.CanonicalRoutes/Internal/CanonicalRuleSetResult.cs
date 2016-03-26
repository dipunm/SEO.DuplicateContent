using System;
using ReturnNull.CanonicalRoutes.Models;

namespace ReturnNull.CanonicalRoutes.Internal
{
    internal class CanonicalRuleSetResult
    {
        public CanonicalRuleSetResult(bool shouldRedirect, UrlPlan plan)
        {
            ShouldRedirect = shouldRedirect;
            Plan = plan;
        }

        public bool ShouldRedirect { get; }
        public UrlPlan Plan { get; }
    }
}