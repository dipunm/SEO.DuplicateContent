using System.Web.Mvc;
using System.Web.Routing;

namespace ReturnNull.CanonicalRoutes.Mvc.RouteConstraints
{
    public static class CanonicalRouteExtensions
    {
        public static void MapMvcAttributeRoutes(this RouteCollection routes,
            ICanonicalConstraintResolver constraintResolver)
        {
            routes.MapMvcAttributeRoutes(new CanonicallyConstrainedDirectRouteProvider(constraintResolver));
        }
    }

}