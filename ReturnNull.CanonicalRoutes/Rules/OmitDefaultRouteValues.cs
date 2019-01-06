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

            var virtualPath = requestData.Route.GetVirtualPath(
                requestData.HttpContext.Request.RequestContext,
                requestData.RouteValues);

            if (virtualPath == null)
            {
                // unable to validate url, skip...
                return false;
            }

            var generatedPath = "/" + virtualPath.VirtualPath;

            return !urlPath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries)
                .SequenceEqual(generatedPath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries), 
                StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
