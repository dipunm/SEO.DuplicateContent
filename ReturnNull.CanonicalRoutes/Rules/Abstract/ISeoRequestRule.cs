using ReturnNull.CanonicalRoutes.Models;

namespace ReturnNull.CanonicalRoutes.Rules.Abstract
{
    public interface ISeoRequestRule
    {
        bool HasBeenViolated(RequestData requestData, UserProvisions provisions);
        void CorrectPlan(UrlPlan plan, RequestData requestData, UserProvisions provisions);
    }
}