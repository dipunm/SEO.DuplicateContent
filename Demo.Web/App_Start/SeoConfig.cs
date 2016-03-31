using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReturnNull.CanonicalRoutes.Configuration;
using ReturnNull.CanonicalRoutes.Rules;

namespace Demo.Web.App_Start
{
    public class SeoConfig
    {
        public static void SetupDuplicateContentRules(SeoRequestRulesetCollection rules)
        {
            rules.Add("Default", SeoRequestRuleset.Recommended());

            var sslRuleset = SeoRequestRuleset.Recommended();
            sslRuleset.RedirectRules.Add(new EnforceScheme("https"));
            rules.Add("SSL", sslRuleset);
        }
    }
}