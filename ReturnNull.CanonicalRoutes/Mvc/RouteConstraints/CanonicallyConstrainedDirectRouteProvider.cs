using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using ReturnNull.CanonicalRoutes.Internal;

namespace ReturnNull.CanonicalRoutes.Mvc.RouteConstraints
{
    /// <summary>
    /// Extends the DefaultDirectRouteProvider and applies appropriate IRouteConstraints to the 
    /// provided Routes using the supplied instance of ICanonicalConstraintResolver.
    /// </summary>
    public class CanonicallyConstrainedDirectRouteProvider : DefaultDirectRouteProvider
    {
        private readonly ICanonicalConstraintResolver _constraintResolver;

        public CanonicallyConstrainedDirectRouteProvider(ICanonicalConstraintResolver constraintResolver)
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
                    foreach (var constraint in _constraintResolver.GetNeutralRouteConstraints(entry, actionDescriptor))
                    {
                        entry.Route.Constraints.Add(constraint.Key, constraint.Value);
                    }
                }
                else if (isCanonicalRoute(entry))
                {
                    foreach (var constraint in _constraintResolver.GetCanonicalRouteConstraints(entry, actionDescriptor))
                    {
                        entry.Route.Constraints.Add(constraint.Key, constraint.Value);
                    }
                }
                else
                {
                    foreach (var constraint in _constraintResolver.GetLegacyRouteConstraints(entry, actionDescriptor))
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
                    foreach (var constraint in _constraintResolver.GetNeutralRouteConstraints(entry, actionDescriptors.ToArray()))
                    {
                        entry.Route.Constraints.Add(constraint.Key, 
                            new TargetedConstraint( new List<Tuple<string, string>>(actionsWhereNeutral
                                    .Select(a => new Tuple<string, string>(a.ActionDescriptor.ControllerDescriptor.ControllerName, a.ActionDescriptor.ActionName))), 
                                constraint.Value));
                    }
                }

                if (actionsWhereCanonical.Any())
                {
                    foreach (var constraint in _constraintResolver.GetCanonicalRouteConstraints(entry, actionDescriptors.ToArray()))
                    {
                        entry.Route.Constraints.Add(constraint.Key,
                            new TargetedConstraint(new List<Tuple<string, string>>(actionsWhereCanonical
                                    .Select(a => new Tuple<string, string>(a.ActionDescriptor.ControllerDescriptor.ControllerName, a.ActionDescriptor.ActionName))), 
                                constraint.Value));
                    }
                }

                if (actionsWhereLegacy.Any())
                {
                    foreach (var constraint in _constraintResolver.GetLegacyRouteConstraints(entry, actionDescriptors.ToArray()))
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
}