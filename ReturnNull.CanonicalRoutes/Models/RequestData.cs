using System;
using System.Web;
using System.Web.Routing;

namespace ReturnNull.CanonicalRoutes.Models
{
    public class RequestData
    {
        public RequestData(HttpContextBase httpContext, RouteBase route, RouteValueDictionary routeValues,
            Uri requestUri)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            if (route == null) throw new ArgumentNullException(nameof(route));
            if (routeValues == null) throw new ArgumentNullException(nameof(routeValues));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));
            HttpContext = httpContext;
            Route = route;
            RouteValues = routeValues;
            RequestUri = requestUri;
        }
        
        public Uri RequestUri { get; }
        public HttpContextBase HttpContext { get; }
        public RouteBase Route { get; }
        public RouteValueDictionary RouteValues { get; }
    }
}