using System;

namespace ReturnNull.CanonicalRoutes.Mvc
{
    public class CanonicalAttribute : Attribute
    {
        public CanonicalAttribute()
        {
            Sensitive = new string[0];
            Query = new string[0];
        }
        /// <summary>
        /// The name of the canonical definition to use when deciding whether to redirect or not.
        /// </summary>
        public string Ruleset { get; set; }

        /// <summary>
        /// A list of all route or querystring parameters that would change the page content if lowercased.
        /// </summary>
        public string[] Sensitive { get; set; }

        /// <summary>
        /// A list of querystring parameters that can significantly change the page's content.
        /// </summary>
        public string[] Query { get; set; }
    }

}