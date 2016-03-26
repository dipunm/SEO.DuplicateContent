using ReturnNull.CanonicalRoutes.Models;
using Shouldly;

namespace ReturnNull.CanonicalRoutes.Tests
{
    public static class UrlPlanHelper
    {
        public static void ShouldMatch(this UrlPlan first, UrlPlan second)
        {
            first.Authority.ShouldBe(second.Authority);
            first.Values.ShouldBe(second.Values);
            first.Query.ShouldBe(second.Query);
        }
    }
}