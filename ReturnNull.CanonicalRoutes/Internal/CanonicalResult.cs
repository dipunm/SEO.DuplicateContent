using System;

namespace ReturnNull.CanonicalRoutes.Internal
{
    internal class CanonicalResult
    {
        public bool ShouldRedirect { get; set; }
        public Uri Url { get; set; }
    }
}