using System;

namespace ReturnNull.CanonicalRoutes.Internal
{
    internal class CanonicalRuleSetResult
    {
        public bool ShouldRedirect { get; set; }
        public Uri Url { get; set; }
    }
}