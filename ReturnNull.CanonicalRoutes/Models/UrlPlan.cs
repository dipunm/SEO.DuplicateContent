using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Models
{
    public class UrlPlan
    {
        public UrlPlan(RequestData requestData)
        {
            Query = requestData.RequestUri.Query;
            Authority = requestData.RequestUri.GetLeftPart(UriPartial.Authority);
            Values = new RouteValueDictionary(requestData.RouteValues);
            Route = requestData.Route;
        }
        public RouteValueDictionary Values { get; set; }
        public string Query { get; set; }
        public string Authority { get; set; }
        public RouteBase Route { get; set; }

        public Uri GenerateUrl(HttpContextBase httpContext)
        {
            var correctPath = Route
                .GetVirtualPath(httpContext.Request.RequestContext, Values)
                ?.VirtualPath;
            if (correctPath == null)
                throw new InvalidOperationException("Unable to generate redirect url.");

            return new UriBuilder(Authority)
            {
                Path = correctPath,
                Query = Query,
            }.Uri;
        }
    }
}