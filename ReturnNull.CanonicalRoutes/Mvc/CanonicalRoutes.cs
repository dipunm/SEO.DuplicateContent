using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;
using ReturnNull.CanonicalRoutes.Internal;

namespace ReturnNull.CanonicalRoutes.Mvc
{
    public static class CanonicalRoutes
    {
        public static void MapMvcAttributeRoutes(this RouteCollection routes,
            ICanonicalConstraintResolver constraintResolver)
        {
            routes.MapMvcAttributeRoutes(new CanonicalRouteProvider(constraintResolver));
        }
    }

    public class CanonicalRouteProvider : DefaultDirectRouteProvider
    {
        private readonly ICanonicalConstraintResolver _constraintResolver;

        public CanonicalRouteProvider(ICanonicalConstraintResolver constraintResolver)
        {
            _constraintResolver = constraintResolver;
        }

        protected override IReadOnlyList<RouteEntry> GetActionDirectRoutes(ActionDescriptor actionDescriptor,
            IReadOnlyList<IDirectRouteFactory> factories,
            IInlineConstraintResolver constraintResolver)
        {
            var routes = base.GetActionDirectRoutes(actionDescriptor, factories, constraintResolver);
            var canonicalRouteName = AttributeHelper.GetCanonicalSettings(actionDescriptor)?.RouteName;

            var isNeutralRoute = string.IsNullOrEmpty(canonicalRouteName);
            var isCanonicalRoute =
                (Func<RouteEntry, bool>)
                (entry => canonicalRouteName?.Equals(entry.Name, StringComparison.OrdinalIgnoreCase) ?? false);

            foreach (var entry in routes)
            {
                if (isNeutralRoute)
                {
                    foreach (var constraint in _constraintResolver.NeutralRouteConstraints)
                    {
                        entry.Route.Constraints.Add(constraint.Key, constraint.Value);
                    }
                }
                else if (isCanonicalRoute(entry))
                {
                    foreach (var constraint in _constraintResolver.CanonicalRouteConstraints)
                    {
                        entry.Route.Constraints.Add(constraint.Key, constraint.Value);
                    }
                }
                else
                {
                    foreach (var constraint in _constraintResolver.LegacyRouteConstraints)
                    {
                        entry.Route.Constraints.Add(constraint.Key, constraint.Value);
                    }
                }
            }
            return routes;
        }

        protected override IReadOnlyList<RouteEntry> GetControllerDirectRoutes(
            ControllerDescriptor controllerDescriptor, IReadOnlyList<ActionDescriptor> actionDescriptors,
            IReadOnlyList<IDirectRouteFactory> factories, IInlineConstraintResolver constraintResolver)
        {
            var routes = base.GetControllerDirectRoutes(controllerDescriptor, actionDescriptors, factories,
                constraintResolver);
            var canonicalActions = actionDescriptors.Select(actionDescriptor => new
            {
                ActionDescriptor = actionDescriptor,
                AttributeHelper.GetCanonicalSettings(actionDescriptor)?.RouteName
            }).ToArray();
            
            var actionsWhereNeutral = canonicalActions.Where(a =>
                        string.IsNullOrEmpty(a.RouteName)).ToArray();

            foreach (var entry in routes)
            {
                var actionsWhereCanonical = canonicalActions.Where(a =>
                    !string.IsNullOrEmpty(a.RouteName) &&
                    a.RouteName.Equals(entry.Name, StringComparison.OrdinalIgnoreCase)).ToArray();

                var actionsWhereLegacy = canonicalActions.Where(a =>
                    !string.IsNullOrEmpty(a.RouteName) &&
                    !a.RouteName.Equals(entry.Name, StringComparison.OrdinalIgnoreCase)).ToArray();

                if (actionsWhereNeutral.Any())
                {
                    foreach (var constraint in _constraintResolver.NeutralRouteConstraints)
                    {
                        entry.Route.Constraints.Add(constraint.Key, 
                            new TargetedConstraint( new List<Tuple<string, string>>(actionsWhereNeutral
                            .Select(a => new Tuple<string, string>(a.ActionDescriptor.ControllerDescriptor.ControllerName, a.ActionDescriptor.ActionName))), 
                            constraint.Value));
                    }
                }

                if (actionsWhereCanonical.Any())
                {
                    foreach (var constraint in _constraintResolver.CanonicalRouteConstraints)
                    {
                        entry.Route.Constraints.Add(constraint.Key,
                            new TargetedConstraint(new List<Tuple<string, string>>(actionsWhereCanonical
                            .Select(a => new Tuple<string, string>(a.ActionDescriptor.ControllerDescriptor.ControllerName, a.ActionDescriptor.ActionName))), 
                            constraint.Value));
                    }
                }

                if (actionsWhereLegacy.Any())
                {
                    foreach (var constraint in _constraintResolver.LegacyRouteConstraints)
                    {
                        entry.Route.Constraints.Add(constraint.Key,
                            new TargetedConstraint(new List<Tuple<string, string>>(actionsWhereLegacy
                            .Select(a => new Tuple<string, string>(a.ActionDescriptor.ControllerDescriptor.ControllerName, a.ActionDescriptor.ActionName))), 
                            constraint.Value));
                    }
                }
            }
            return routes;
        }
    }

    public class TargetedConstraint : IRouteConstraint
    {
        private readonly object _constraint;
        private readonly ICollection<Tuple<string, string>> _controllerActions;

        public TargetedConstraint(ICollection<Tuple<string, string>> controllerActions, object constraint)
        {
            _constraint = constraint;
            _controllerActions = controllerActions;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if(!_controllerActions.Any(pair => 
                pair.Item1.Equals(values["controller"].ToString(), StringComparison.OrdinalIgnoreCase) &&
                pair.Item2.Equals(values["action"].ToString(), StringComparison.OrdinalIgnoreCase)))
                return true; //do not constrain.

            var customConstraint = GetCustomConstraint(route, parameterName);
            return customConstraint.Match(httpContext, route, parameterName, values, routeDirection);
        }

        private IRouteConstraint GetCustomConstraint(Route route, string parameterName)
        {
            var customConstraint = _constraint as IRouteConstraint;
            if (customConstraint == null)
            {
                string constraintsRule = _constraint as string;
                if (constraintsRule == null)
                {
                    throw new InvalidOperationException(
                        $"Invalid route constraint for parameter '{parameterName}' on route with pattern '{route.Url}'. " +
                        $"Route constraints must be a string or a custom constraint of type ICustomConstraint.");
                }

                customConstraint = new RegexRouteConstraint("^(" + constraintsRule + ")$");
            }
            return customConstraint;
        }
    }
}