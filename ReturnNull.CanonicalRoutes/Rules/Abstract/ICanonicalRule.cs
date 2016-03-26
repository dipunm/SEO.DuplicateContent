using System;
using System.Collections.Generic;
using ReturnNull.CanonicalRoutes.Models;

namespace ReturnNull.CanonicalRoutes.Rules.Abstract
{
    public interface ICanonicalRule
    {
        bool HasBeenViolated(RequestData requestData, UserProvisions provisions);
        void CorrectPlan(UrlPlan plan, RequestData requestData, UserProvisions provisions);
    }

    public class UserProvisions
    {
        public UserProvisions(
            IEnumerable<string> sensitiveParameters, 
            IEnumerable<string> canonicalQuerystrings)
        {
            if (sensitiveParameters == null) throw new ArgumentNullException(nameof(sensitiveParameters));
            if (canonicalQuerystrings == null) throw new ArgumentNullException(nameof(canonicalQuerystrings));
            SensitiveParameters = sensitiveParameters;
            CanonicalQuerystrings = canonicalQuerystrings;
        }

        public IEnumerable<string> SensitiveParameters { get; }
        public IEnumerable<string> CanonicalQuerystrings { get; }
    }
}