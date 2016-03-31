using System;
using System.Linq;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class OmitDefaultRouteValues : SeoRequestRuleSnitch
    {
        public override bool HasBeenViolated(RequestData requestData, UserProvisions provisions)
        {
            var urlPath = requestData.RequestUri.AbsolutePath;
            if (urlPath == "/") return false;

            var generatedPath = "/" + requestData.Route.GetVirtualPath(
                requestData.HttpContext.Request.RequestContext,
                requestData.RouteValues)?.VirtualPath;

            if (string.IsNullOrEmpty(generatedPath))
                throw new InvalidOperationException("Provided route was unable to generate a url from the given parameters.");

            return !urlPath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries)
                .SequenceEqual(generatedPath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries), 
                StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
