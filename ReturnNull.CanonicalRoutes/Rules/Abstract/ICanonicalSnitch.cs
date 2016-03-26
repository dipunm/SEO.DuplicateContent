using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReturnNull.CanonicalRoutes.Models;

namespace ReturnNull.CanonicalRoutes.Rules.Abstract
{
    public abstract class CanonicalSnitch : ICanonicalRule
    {
        public abstract bool HasBeenViolated(Uri url, RouteInfo routeInfo, UserProvisions provisions);

        //non-overridable NOP method.
        public void CorrectPlan(UrlPlan plan, RouteInfo routeInfo, UserProvisions provisions){}
    }
}
