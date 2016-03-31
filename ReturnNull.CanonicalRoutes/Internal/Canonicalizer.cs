using System.Linq;
using ReturnNull.CanonicalRoutes.Configuration;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Internal
{
    internal class Canonicalizer
    {
        private readonly SeoRequestRuleset _ruleset;

        public Canonicalizer(SeoRequestRuleset ruleset)
        {
            _ruleset = ruleset;
        }

        public CanonicalRulesetResult Canonicalize(RequestData requestData, UserProvisions provisions)
        {
            var plan = new UrlPlan(requestData);

            var shouldRedirect = _ruleset.RedirectRules
                .Any(rule => rule.HasBeenViolated(requestData, provisions));

            var rulesToApply = shouldRedirect ? _ruleset.RedirectRules : _ruleset.RewriteRules;
            foreach (var rule in rulesToApply)
            {
                rule.CorrectPlan(plan, requestData, provisions);
            }

            return new CanonicalRulesetResult(shouldRedirect, plan);
        }
    }
}