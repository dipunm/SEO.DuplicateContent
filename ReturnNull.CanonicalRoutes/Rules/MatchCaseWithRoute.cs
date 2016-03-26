using System;
using System.Linq;
using System.Web.Routing;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class MatchCaseWithRoute : ICanonicalRule {
        public bool HasBeenViolated(Uri url, RouteInfo routeInfo, UserProvisions provisions)
        {
            var urlPath = url.AbsolutePath;
            if (urlPath == "/") return false;

            var loweredRouteValues = new RouteValueDictionary();
            foreach (var routeValue in routeInfo.RouteValues)
            {
                var sensitive = provisions.SensitiveParameters.Contains(routeValue.Key,
                    StringComparer.InvariantCultureIgnoreCase) || routeValue.Value.GetType() != typeof(string);
                loweredRouteValues.Add(routeValue.Key, sensitive ? routeValue.Value : routeValue.Value.ToString().ToLowerInvariant());
            }

            var generatedPath = "/" + routeInfo.Route.GetVirtualPath(
                routeInfo.HttpContext.Request.RequestContext,
                loweredRouteValues)?.VirtualPath;

            if (string.IsNullOrEmpty(generatedPath))
                throw new InvalidOperationException("Provided route was unable to generate a url from the given parameters.");

            generatedPath = generatedPath.Substring(0, Math.Min(urlPath.Length, generatedPath.Length));
            urlPath = urlPath.Substring(0, Math.Min(urlPath.Length, generatedPath.Length));

            return !generatedPath.Equals(urlPath, StringComparison.Ordinal);
        }

        public void CorrectPlan(UrlPlan plan, RouteInfo routeInfo, UserProvisions provisions)
        {
            var loweredRouteValues = new RouteValueDictionary();
            foreach (var routeValue in plan.Values)
            {
                var sensitive = provisions.SensitiveParameters.Contains(routeValue.Key,
                    StringComparer.InvariantCultureIgnoreCase) || routeValue.Value.GetType() != typeof(string);
                loweredRouteValues.Add(routeValue.Key, sensitive ? routeValue.Value : routeValue.Value.ToString().ToLowerInvariant());
            }

            plan.Values = loweredRouteValues;
        }
    }
}