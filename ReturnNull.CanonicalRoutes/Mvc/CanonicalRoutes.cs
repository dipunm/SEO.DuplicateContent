using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;
using ReturnNull.CanonicalRoutes.Internal;

namespace ReturnNull.CanonicalRoutes.Mvc
{
    public static class CanonicalRoutes
    {
        public static void MapMvcAttributeRoutes(this RouteCollection routes, Include routeTypes)
        {
            routeTypes = routeTypes & Include.All;
            if (routeTypes == 0 || routeTypes == Include.All)
                //default behaviour
                routes.MapMvcAttributeRoutes();
            else
            {
                   routes.MapMvcAttributeRoutes(new CanonicalRouteProvider(routeTypes));
            }
        }
    }

    public class CanonicalRouteProvider : DefaultDirectRouteProvider
    {
        private readonly Include _routeTypes;

        public CanonicalRouteProvider(Include routeTypes)
        {
            _routeTypes = routeTypes;
        }

        protected override IReadOnlyList<RouteEntry> GetActionDirectRoutes(ActionDescriptor actionDescriptor, 
            IReadOnlyList<IDirectRouteFactory> factories,
            IInlineConstraintResolver constraintResolver)
        {
            var routes = base.GetActionDirectRoutes(actionDescriptor, factories, constraintResolver);
            var canonicalRouteName = AttributeHelper.GetCanonicalSettings(actionDescriptor)?.RouteName;

            if (string.IsNullOrEmpty(canonicalRouteName))
            {
                return _routeTypes.HasFlag(Include.NeutralActionRoutes) ?
                    routes : new RouteEntry[0];
            }

            if (_routeTypes.HasFlag(Include.CanonicalActionRoutes) && _routeTypes.HasFlag(Include.LegacyActionRoutes))
            {
                return routes;
            }

            return routes.Where(entry =>
                _routeTypes.HasFlag(Include.CanonicalActionRoutes) == RouteIsCanonical(canonicalRouteName, entry))
                .ToArray();
        }

        protected override IReadOnlyList<RouteEntry> GetControllerDirectRoutes(ControllerDescriptor controllerDescriptor, IReadOnlyList<ActionDescriptor> actionDescriptors,
            IReadOnlyList<IDirectRouteFactory> factories, IInlineConstraintResolver constraintResolver)
        {
            var routes = base.GetControllerDirectRoutes(controllerDescriptor, actionDescriptors, factories, constraintResolver);
            var canonicalRouteNames = actionDescriptors
                .Select(desc => AttributeHelper.GetCanonicalSettings(desc)?.RouteName)
                .Where(name => name!=null)
                .ToList();

            /*
             match when:
             canonical && canonical_allowed
             !canonical && neutral && neutral_allowed
             !canonical && !neutral && legacy_allowed
             */

            return routes.Where(r =>
                (_routeTypes.HasFlag(Include.CanonicalActionRoutes)
                 && canonicalRouteNames.Contains(r.Name, StringComparer.OrdinalIgnoreCase))
                ||
                (_routeTypes.HasFlag(Include.NeutralActionRoutes)
                 && !canonicalRouteNames.Any())
                ||
                (_routeTypes.HasFlag(Include.LegacyActionRoutes)
                 && canonicalRouteNames.Any()
                 && !canonicalRouteNames.Contains(r.Name, StringComparer.OrdinalIgnoreCase))
                )
                .ToArray();
        }

        private static bool RouteIsCanonical(string canonicalRoute, RouteEntry entry)
            => canonicalRoute.Equals(entry.Name, StringComparison.OrdinalIgnoreCase);
    }
    
    [Flags]
    public enum Include
    {
        /// <summary>
        /// Named routes that matches the CanonicalRoute defined by any action.
        /// </summary>
        CanonicalActionRoutes = 1,

        /// <summary>
        /// Named or unnamed routes that serve actions for which a 
        /// CanonicalRoute has been defined, and where none of these
        /// actions name the route.
        /// </summary>
        LegacyActionRoutes = 2,

        /// <summary>
        /// Named or unnamed routes that only serve actions for which a
        /// CanonicalRoute han not been defined.
        /// </summary>
        NeutralActionRoutes = 4,

        All = CanonicalActionRoutes | LegacyActionRoutes | NeutralActionRoutes
    }
}