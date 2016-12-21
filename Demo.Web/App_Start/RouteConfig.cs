using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ReturnNull.CanonicalRoutes.Mvc;
using ReturnNull.CanonicalRoutes.Mvc.RouteConstraints;

namespace Demo.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapMvcAttributeRoutes(new CanonicalConstraintResolver()
            {
                CanonicalRouteConstraints = { { "canonical", new QsMatchIfPresentConstraint("true") } },
                NeutralRouteConstraints = { { "neutral", new QsMatchIfPresentConstraint("true") } },
                LegacyRouteConstraints = { { "legacy", new QsMatchIfPresentConstraint("true") } }
            });

            //preferred route for the homepage
            routes.MapRoute(
                name: "another",
                url: "another",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }

    public class QsMatchIfPresentConstraint : IRouteConstraint
    {
        private readonly string _value;

        public QsMatchIfPresentConstraint(string value)
        {
            _value = value;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            return !httpContext.Request.QueryString.AllKeys.Contains(parameterName) 
                || httpContext.Request.QueryString[parameterName].Equals(_value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
