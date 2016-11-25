using System.Linq;
using System.Web.Mvc;
using ReturnNull.CanonicalRoutes.Configuration;
using ReturnNull.CanonicalRoutes.Helpers;
using ReturnNull.CanonicalRoutes.Internal;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Mvc
{
    public class HandleDuplicateContentFilter : IActionFilter
    {
        internal const string CanonicalDataKey = "canonicalUrl";

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //child actions are not supported
            if (filterContext.IsChildAction)
                return;

            var settings = AttributeHelper.GetCanonicalSettings(filterContext.ActionDescriptor);
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

            var ruleset = SeoRequestRulesetCollection.Rules.Get(settings.Ruleset);
            var canonicalizer = new Canonicalizer(ruleset);
            var result = canonicalizer.Canonicalize(requestData, new UserProvisions(settings.Sensitive, settings.Query, settings.RouteName));
            var newUrl = result.Plan.GenerateUrl(httpContext).ToString();
            if (result.ShouldRedirect)
                filterContext.Result = new RedirectResult(newUrl, true);
            else
                filterContext.RouteData.DataTokens.Add(CanonicalDataKey, newUrl);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}