using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using ReturnNull.CanonicalRoutes.Models;

namespace ReturnNull.CanonicalRoutes.Rules.Abstract
{
    public interface ICanonicalRule
    {
        bool HasBeenViolated(Uri url, RouteInfo routeInfo, UserProvisions provisions);
        void CorrectPlan(UrlPlan plan, RouteInfo routeInfo, UserProvisions provisions);
    }

    public class UserProvisions
    {
        public UserProvisions(
            IEnumerable<string> sensitiveParameters, 
            IEnumerable<string> canonicalQuerystrings)
        {
            if (sensitiveParameters == null) throw new ArgumentNullException(nameof(sensitiveParameters));
            if (canonicalQuerystrings == null) throw new ArgumentNullException(nameof(canonicalQuerystrings));
            SensitiveParameters = sensitiveParameters;
            CanonicalQuerystrings = canonicalQuerystrings;
        }

        public IEnumerable<string> SensitiveParameters { get; }
        public IEnumerable<string> CanonicalQuerystrings { get; }
    }

    public class RouteInfo
    {
        public RouteInfo(HttpContextBase httpContext, RouteBase route, RouteValueDictionary routeValues)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            if (route == null) throw new ArgumentNullException(nameof(route));
            if (routeValues == null) throw new ArgumentNullException(nameof(routeValues));
            HttpContext = httpContext;
            Route = route;
            RouteValues = routeValues;
        }

        public HttpContextBase HttpContext { get; }
        public RouteBase Route { get; }
        public RouteValueDictionary RouteValues { get; }
    }
}