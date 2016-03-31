using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReturnNull.CanonicalRoutes.Models;

namespace ReturnNull.CanonicalRoutes.Rules.Abstract
{
    /// <summary>
    /// Helper class, defines a rule that can detect issues with the request, 
    /// but needs to do nothing to resolve it
    /// </summary>
    public abstract class SeoRequestRuleSnitch : ISeoRequestRule
    {
        public abstract bool HasBeenViolated(RequestData requestData, UserProvisions provisions);

        //non-overridable NOP method.
        public void CorrectPlan(UrlPlan plan, RequestData requestData, UserProvisions provisions){}
    }
}
