using System;
using System.Web;
using System.Web.Routing;

namespace ReturnNull.CanonicalRoutes.Models
{
    public class UrlPlan
    {
        public RouteValueDictionary Values { get; set; }
        public string Query { get; set; }
        public string Fragment { get; set; }
        public string Authority { get; set; }

        public Uri Execute(HttpContextBase httpContext, RouteBase route)
        {
            var correctPath = route
                .GetVirtualPath(httpContext.Request.RequestContext, Values)
                ?.VirtualPath;
            if (correctPath == null)
                throw new InvalidOperationException("Unable to generate redirect url.");

            return new UriBuilder(Authority)
            {
                Path = correctPath,
                Query = Query,
                Fragment = Fragment
            }.Uri;
        }
    }
}