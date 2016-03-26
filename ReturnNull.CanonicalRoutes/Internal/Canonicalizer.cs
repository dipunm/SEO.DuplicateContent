using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Internal
{
    internal class Canonicalizer
    {
        private readonly ICollection<ICanonicalRule> _redirectRules;
        private readonly ICollection<ICanonicalRule> _rewriteRules;
        private readonly string[] _sensitiveParameters;
        private readonly string[] _canonicalQuerystrings;

        public Canonicalizer(ICollection<ICanonicalRule> rewriteRules, ICollection<ICanonicalRule> redirectRules, 
            string[] sensitiveParameters = null, string[] canonicalQuerystrings = null)
        {
            _sensitiveParameters = sensitiveParameters ?? new string[0];
            _canonicalQuerystrings = canonicalQuerystrings ?? new string[0];
            _redirectRules = redirectRules;
            _rewriteRules = rewriteRules;
        }

        public CanonicalRuleSetResult Canonicalize(HttpContextBase httpContext, RouteData routeData)
        {
            var originalUrl = new UriBuilder(
                $"{httpContext.Request.Url?.Scheme}://" +
                $"{httpContext.Request.ServerVariables["HTTP_HOST"]}" +
                $"{httpContext.Request.ServerVariables["HTTP_URL"]}" +
                $"{httpContext.Request.ServerVariables["HTTP_QUERY"]}").Uri;

            var routeInfo = new RouteInfo(httpContext, routeData.Route, routeData.Values);
            var provisions = new UserProvisions(_sensitiveParameters, _canonicalQuerystrings);
            var plan = new UrlPlan
            {
                Authority = originalUrl.GetLeftPart(UriPartial.Authority),
                Values = routeData.Values,
                Query = originalUrl.Query,
                Fragment = originalUrl.Fragment
            };

            var shouldRedirect = _redirectRules.Any(rule =>
                rule.HasBeenViolated(originalUrl, routeInfo, provisions));

            var canonicalRules = shouldRedirect ? _redirectRules : _rewriteRules;
            foreach (var rule in canonicalRules)
            {
                rule.CorrectPlan(plan, routeInfo, provisions);
            }

            return new CanonicalRuleSetResult
            {
                Url = plan.Execute(httpContext, routeData.Route),
                ShouldRedirect = shouldRedirect
            };
        }
    }
}