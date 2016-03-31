using System.Web;
using System.Web.Mvc;
using ReturnNull.CanonicalRoutes.Mvc;

namespace Demo.Web
{
        public class FilterConfig
        {
            public static void RegisterGlobalFilters(GlobalFilterCollection filters)
            {
                filters.Add(new HandleErrorAttribute());
                filters.Add(new HandleDuplicateContentFilter());
            }
        }
}
