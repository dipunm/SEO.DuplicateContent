using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReturnNull.CanonicalRoutes.Rules;
using ReturnNull.CanonicalRoutes.Rules.Abstract;
using ReturnNull.CanonicalRoutes.Rules.Dependencies;

namespace ReturnNull.CanonicalRoutes.Configuration
{
    public class CanonicalRuleSet
    {
        public virtual IList<ICanonicalRule> RedirectRules { get; } = new List<ICanonicalRule>();
        public virtual IList<ICanonicalRule> RewriteRules { get; } = new List<ICanonicalRule>();
        

        public static CanonicalRuleSet Recommended(Uri preferredAuthority = null, ISlugProvider slugProvider = null)
        {
            var recommended = new CanonicalRuleSet
            {
                RedirectRules =
                {
                    new LowercaseQuerystringKeys(),
                    new LowercaseQuerystringValues(),
                    new MatchCaseWithRoute(),
                    new OmitDefaultRouteValues(),
                    new RemoveRepeatingSlashes(),
                    new RemoveTrailingSlash()
                },
                RewriteRules =
                {
                    new RemoveUncanonicalQuerystrings(),
                    new OrderQuerystrings()
                }
            };

            if(slugProvider != null)
                recommended.RewriteRules.Add(new EnforceCorrectSlug(slugProvider));

            if (preferredAuthority != null)
            {
                recommended.RedirectRules.Add(new EnforceHost(
                    preferredAuthority.Host, 
                    preferredAuthority.IsDefaultPort ? (int?)null : preferredAuthority.Port));
                recommended.RedirectRules.Add(new EnforceScheme(preferredAuthority.Scheme));
            }

            return recommended;
        }
    }
}