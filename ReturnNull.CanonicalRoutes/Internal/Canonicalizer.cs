using System.Linq;
using ReturnNull.CanonicalRoutes.Configuration;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Internal
{
    internal class Canonicalizer
    {
        private readonly CanonicalRuleSet _ruleSet;

        public Canonicalizer(CanonicalRuleSet ruleSet)
        {
            _ruleSet = ruleSet;
        }

        public CanonicalRuleSetResult Canonicalize(RequestData requestData, UserProvisions provisions)
        {
            var plan = new UrlPlan(requestData);

            var shouldRedirect = _ruleSet.RedirectRules
                .Any(rule => rule.HasBeenViolated(requestData, provisions));

            var rulesToApply = shouldRedirect ? _ruleSet.RedirectRules : _ruleSet.RewriteRules;
            foreach (var rule in rulesToApply)
            {
                rule.CorrectPlan(plan, requestData, provisions);
            }

            return new CanonicalRuleSetResult(shouldRedirect, plan);
        }
    }
}