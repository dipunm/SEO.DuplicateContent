using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReturnNull.CanonicalRoutes.Configuration;
using ReturnNull.CanonicalRoutes.Rules;

namespace Routes.App_Start
{
    public class CanonicalConfig
    {
        public static void SetupCanonicalRules(CanonicalRulesetCollection rules)
        {
            rules.Add("Default", CanonicalRuleset.Recommended(new Uri("http://localhost")));
        }
    }
}