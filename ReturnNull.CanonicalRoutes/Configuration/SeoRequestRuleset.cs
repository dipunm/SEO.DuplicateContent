using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReturnNull.CanonicalRoutes.Rules;
using ReturnNull.CanonicalRoutes.Rules.Abstract;
using ReturnNull.CanonicalRoutes.Rules.Dependencies;

namespace ReturnNull.CanonicalRoutes.Configuration
{
    public class SeoRequestRuleset
    {
        public virtual IList<ISeoRequestRule> RedirectRules { get; } = new List<ISeoRequestRule>();
        public virtual IList<ISeoRequestRule> RewriteRules { get; } = new List<ISeoRequestRule>();


        public static SeoRequestRuleset Recommended(Uri preferredAuthority = null)
        {
            var recommended = new SeoRequestRuleset
            {
                RedirectRules =
                {
                    new EnforceCorrectRoute(),
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