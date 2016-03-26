using System.Web.Mvc;

namespace Routes.Route
{
    public static class CanonicalUrlExtensions
    {
        public static string Canonical(this UrlHelper urlHelper)
        {
            return urlHelper.RequestContext.RouteData.DataTokens["canonicalUrl"] as string;
        }
    }
}