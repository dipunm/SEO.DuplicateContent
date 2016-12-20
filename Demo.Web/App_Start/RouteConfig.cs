using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ReturnNull.CanonicalRoutes.Mvc;

namespace Demo.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapMvcAttributeRoutes(new CanonicalConstraintResolver()
            {
                CanonicalRouteConstraints = {{ "canonical", "true" }},
                NeutralRouteConstraints = {{ "neutral", "true" }},
                LegacyRouteConstraints = {{ "legacy", "true" }}
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
}
