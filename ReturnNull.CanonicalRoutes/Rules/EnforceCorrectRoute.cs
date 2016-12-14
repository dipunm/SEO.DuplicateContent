using System;
using System.Web.Routing;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class EnforceCorrectRoute : ISeoRequestRule
    {
        public bool HasBeenViolated(RequestData requestData, UserProvisions provisions)
        {
            var routeName = provisions.RouteName;
            if (routeName == null) return false;
            var expectedRoute = RouteTable.Routes[routeName];
            return !(
                expectedRoute == requestData.Route ||
                VirtualPathsMatch(expectedRoute, requestData)
            );
        }

        private bool VirtualPathsMatch(RouteBase linkGenerationRoute, RequestData requestData)
        {
            var internalRoute = linkGenerationRoute.GetVirtualPath(
                requestData.HttpContext.Request.RequestContext, 
                requestData.RouteValues)
                ?.Route;

            return internalRoute == requestData.Route;
        }

        public void CorrectPlan(UrlPlan plan, RequestData requestData, UserProvisions provisions)
        {
            var routeName = provisions.RouteName;
            if (routeName == null) return;

            if(RouteTable.Routes[routeName] == null)
                throw new InvalidOperationException(
                    $"Unknown route with name `{routeName}` found. Please check the Canonical attribute on your controller/action.");

            plan.Route = RouteTable.Routes[routeName];            
        }
    }
}
