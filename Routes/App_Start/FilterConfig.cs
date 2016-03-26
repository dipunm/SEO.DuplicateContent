using System.Web;
using System.Web.Mvc;
using ReturnNull.CanonicalRoutes.Mvc;

namespace Routes
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CanonicalFilter());
        }
    }
}
