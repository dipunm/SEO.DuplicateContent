using System.Linq;
using System.Web.Mvc;
using ReturnNull.CanonicalRoutes.Configuration;
using ReturnNull.CanonicalRoutes.Internal;

namespace ReturnNull.CanonicalRoutes.Mvc
{
    public class CanonicalRulesFilter : IActionFilter
    {
        private static CanonicalAttribute ReadCanonicalizeAttributes(ActionExecutingContext filterContext)
        {
            var attributes = 
                filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(CanonicalAttribute), true)
                .Concat(filterContext.ActionDescriptor.GetCustomAttributes(typeof(CanonicalAttribute), true));

            var settings = attributes.Cast<CanonicalAttribute>()
                .Aggregate(new CanonicalAttribute(),(a, b) => new CanonicalAttribute()
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
            var canonicalizer = new Canonicalizer(
                ruleSet.RewriteRules, //new CanonicalRuleTracingCollection
                ruleSet.RedirectRules, //new CanonicalRuleTracingCollection 
                settings.Sensitive, 
                settings.Query);

            return canonicalizer;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var settings = ReadCanonicalizeAttributes(filterContext);
            if (string.IsNullOrEmpty(settings.Ruleset))
                return;

            var canonicalizer = CreateCanonicalizer(settings);
            var result = canonicalizer.Canonicalize(filterContext.RequestContext.HttpContext, filterContext.RequestContext.RouteData);
            if (result.ShouldRedirect)
            {
                filterContext.Result = new RedirectResult(result.Url.ToString(), true);
            }
            else
            {
                filterContext.RouteData.DataTokens.Add("canonicalUrl", result.Url.ToString());
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}