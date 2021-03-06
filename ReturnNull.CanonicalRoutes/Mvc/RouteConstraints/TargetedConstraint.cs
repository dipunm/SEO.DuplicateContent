using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Routing.Constraints;
using System.Web.Routing;

namespace ReturnNull.CanonicalRoutes.Mvc.RouteConstraints
{
    /// <summary>
    /// Wraps a constraint and applies it only if the targeted action and controller
    /// is expected.
    /// </summary>
    public class TargetedConstraint : IRouteConstraint
    {
        private readonly object _constraint;
        private readonly ICollection<Tuple<string, string>> _expectedControllerActionPairs;

        public TargetedConstraint(ICollection<Tuple<string, string>> expectedControllerActionPairs, object constraint)
        {
            _constraint = constraint;
            _expectedControllerActionPairs = expectedControllerActionPairs;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if(!_expectedControllerActionPairs.Any(pair => 
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