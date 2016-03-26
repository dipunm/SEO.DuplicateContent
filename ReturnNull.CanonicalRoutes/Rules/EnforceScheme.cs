using System;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class EnforceScheme : ICanonicalRule
    {
        private readonly string _scheme;
        public const string Https = "https";
        public const string Http = "http";

        public EnforceScheme(string scheme)
        {
            _scheme = scheme;
        }

        public bool HasBeenViolated(Uri url, RouteInfo routeInfo, UserProvisions provisions)
        {
            return url.Scheme != _scheme;
        }

        public void CorrectPlan(UrlPlan plan, RouteInfo routeInfo, UserProvisions provisions)
        {
            var builder = new UriBuilder(plan.Authority)
            {
                Scheme = _scheme
            };
            plan.Authority = builder.Uri.GetLeftPart(UriPartial.Authority);
        }
    }
}