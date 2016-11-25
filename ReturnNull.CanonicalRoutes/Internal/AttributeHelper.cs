using System.Linq;
using System.Web.Mvc;
using ReturnNull.CanonicalRoutes.Mvc;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Internal
{
    public class AttributeHelper
    {
        public static CanonicalAttribute GetCanonicalSettings(ActionDescriptor actionDescriptor)
        {
            var attributes =
                actionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(CanonicalAttribute), true)
                .Concat(actionDescriptor.GetCustomAttributes(typeof(CanonicalAttribute), true))
                .Cast<CanonicalAttribute>().ToList();

            if (!attributes.Any())
                return null;

            var settings = attributes
                .Aggregate((a, b) => new CanonicalAttribute
                {
                    Query = a.Query.Concat(b.Query).Distinct().ToArray(),
                    Ruleset = b.Ruleset ?? a.Ruleset,
                    Sensitive = a.Sensitive.Concat(b.Sensitive).Distinct().ToArray(),
                    RouteName = b.RouteName ?? a.RouteName
                });

            return settings;
        }
    }
}
