using System;
using System.Linq;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class OmitDefaultRouteValues : CanonicalRuleSnitch
    {
        public override bool HasBeenViolated(Uri url, RouteInfo routeInfo, UserProvisions provisions)
        {
            var urlPath = url.AbsolutePath;
            if (urlPath == "/") return false;

            var generatedPath = "/" + routeInfo.Route.GetVirtualPath(
                routeInfo.HttpContext.Request.RequestContext,
                routeInfo.RouteValues)?.VirtualPath;

            if (string.IsNullOrEmpty(generatedPath))
                throw new InvalidOperationException("Provided route was unable to generate a url from the given parameters.");

            return !urlPath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries)
                .SequenceEqual(generatedPath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries), 
                StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
