using System;
using System.Linq;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;

namespace Routes
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "home", action = "index", id = UrlParameter.Optional }
            );

            
        }
    }
}