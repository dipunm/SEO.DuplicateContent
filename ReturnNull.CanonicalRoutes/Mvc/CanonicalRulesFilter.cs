using System.Linq;
using System.Web.Mvc;
using ReturnNull.CanonicalRoutes.Configuration;
using ReturnNull.CanonicalRoutes.Helpers;
using ReturnNull.CanonicalRoutes.Internal;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Mvc
{
    public class CanonicalRulesFilter : IActionFilter
    {
        private static CanonicalAttribute ReadCanonicalizeAttributes(ActionExecutingContext filterContext)
        {
            var attributes =
                filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(CanonicalAttribute), true)
                .Concat(filterContext.ActionDescriptor.GetCustomAttributes(typeof(CanonicalAttribute), true))
                .Cast<CanonicalAttribute>().ToList();

            if (!attributes.Any())
                return null;

            var settings = attributes
                .Aggregate((a, b) => new CanonicalAttribute
                {
                    Query = a.Query.Concat(b.Query).ToArray(),
                    Ruleset = b.Ruleset ?? a.Ruleset,
                    Sensitive = a.Sensitive.Concat(b.Sensitive).ToArray()
                });
            return settings;
        }

        private static Canonicalizer CreateCanonicalizer(CanonicalAttribute settings)
        {
            var ruleSet = CanonicalRuleSetCollection.Rules.Get(settings.Ruleset);
            var canonicalizer = new Canonicalizer(ruleSet);
            return canonicalizer;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var settings = ReadCanonicalizeAttributes(filterContext);
            if (settings == null)
                return;

            var httpContext = filterContext.RequestContext.HttpContext;
            var routeData = filterContext.RequestContext.RouteData;

            var requestData = new RequestData(
                httpContext, 
                routeData.Route, 
                routeData.Values, 
                httpContext.Request.GetRealRequestUri()
            );

            var provisions = new UserProvisions(
                settings.Sensitive, 
                settings.Query
            );

            var canonicalizer = CreateCanonicalizer(settings);
            var result = canonicalizer.Canonicalize(requestData, provisions);
            var newUrl = result.Plan.GenerateUrl(httpContext, routeData.Route).ToString();
            if (result.ShouldRedirect)
            {
                filterContext.Result = new RedirectResult(newUrl, true);
            }
            else
            {
                filterContext.RouteData.DataTokens.Add("canonicalUrl", newUrl);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}