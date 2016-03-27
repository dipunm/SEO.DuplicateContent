using System.Web.Mvc;

namespace ReturnNull.CanonicalRoutes.Mvc
{
    public static class CanonicalUrlExtensions
    {
        public static string Canonical(this UrlHelper urlHelper)
        {
            return urlHelper.RequestContext.RouteData.DataTokens[CanonicalFilter.CanonicalDataKey] as string;
        }
    }
}