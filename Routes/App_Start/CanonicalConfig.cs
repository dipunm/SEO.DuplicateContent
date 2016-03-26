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
        public static void SetupCanonicalRules(CanonicalRuleSetCollection rules)
        {
            rules.Add("Default", CanonicalRuleSet.Recommended(new Uri("http://localhost")));
        }
    }
}