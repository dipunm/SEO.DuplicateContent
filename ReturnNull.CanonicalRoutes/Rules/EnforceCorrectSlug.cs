using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;
using ReturnNull.CanonicalRoutes.Rules.Dependencies;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class EnforceCorrectSlug : ICanonicalRule
    {
        private readonly ISlugProvider _slugProvider;

        public EnforceCorrectSlug(ISlugProvider slugProvider)
        {
            _slugProvider = slugProvider;
        }

        public bool HasBeenViolated(Uri url, RouteInfo routeInfo, UserProvisions provisions)
        {
            return routeInfo.RouteValues.Any(IsWrongSlug);
        }

        private bool IsWrongSlug(KeyValuePair<string, object> parameters)
        {
            if (parameters.Key.EndsWith("slug", StringComparison.InvariantCultureIgnoreCase))
            {
                var correctSlug = _slugProvider.GetSlug(parameters.Key);
                return correctSlug?.Equals(parameters.Value.ToString(), StringComparison.Ordinal) == false;
            }
            return false;
        }

        public void CorrectPlan(UrlPlan plan, RouteInfo routeInfo, UserProvisions provisions)
        {
            var slugParams = plan.Values.Where(IsWrongSlug);
            foreach (var param in slugParams)
            {
                var correctSlug = _slugProvider.GetSlug(param.Key);
                plan.Values[param.Key] = correctSlug;
            }
        }
    }
}