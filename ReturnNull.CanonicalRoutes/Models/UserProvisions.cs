using System;
using System.Collections.Generic;

namespace ReturnNull.CanonicalRoutes.Models
{
    public class UserProvisions
    {
        public UserProvisions(
            IEnumerable<string> sensitiveParameters, 
            IEnumerable<string> canonicalQuerystrings, string routeName)
        {
            if (sensitiveParameters == null) throw new ArgumentNullException(nameof(sensitiveParameters));
            if (canonicalQuerystrings == null) throw new ArgumentNullException(nameof(canonicalQuerystrings));
            SensitiveParameters = sensitiveParameters;
            CanonicalQuerystrings = canonicalQuerystrings;
            RouteName = routeName;
        }

        public IEnumerable<string> SensitiveParameters { get; }
        public IEnumerable<string> CanonicalQuerystrings { get; }
        public string RouteName { get; }
    }
}