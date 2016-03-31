using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;
using ReturnNull.CanonicalRoutes.Rules.Dependencies;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class EnforceCorrectSlug : ISeoRequestRule
    {
        private readonly ISlugProvider _slugProvider;

        public EnforceCorrectSlug(ISlugProvider slugProvider)
        {
            if (slugProvider == null) throw new ArgumentNullException(nameof(slugProvider));
            _slugProvider = slugProvider;
        }

        public bool HasBeenViolated(RequestData requestData, UserProvisions provisions)
        {
            return requestData.RouteValues.Any(val => IsWrongSlug(val, requestData.RouteValues));
        }

        private bool IsWrongSlug(KeyValuePair<string, object> parameters, RouteValueDictionary routeValues)
        {
            if (parameters.Key.EndsWith("slug", StringComparison.InvariantCultureIgnoreCase))
            {
                var correctSlug = _slugProvider.GetSlug(parameters.Key, routeValues.AsEnumerable());
                return correctSlug?.Equals(parameters.Value.ToString(), StringComparison.Ordinal) == false;
            }
            return false;
        }

        public void CorrectPlan(UrlPlan plan, RequestData requestData, UserProvisions provisions)
        {
            var slugParams = plan.Values.Where(parameters => IsWrongSlug(parameters, requestData.RouteValues));
            foreach (var param in slugParams)
            {
                var correctSlug = _slugProvider.GetSlug(param.Key, requestData.RouteValues);
                plan.Values[param.Key] = correctSlug;
            }
        }
    }
}