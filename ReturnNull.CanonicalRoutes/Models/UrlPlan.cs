using System.Web.Routing;

namespace ReturnNull.CanonicalRoutes.Models
{
    public class UrlPlan
    {
        public RouteValueDictionary Values { get; set; }
        public string Query { get; set; }
        public string Fragment { get; set; }
        public string Authority { get; set; }
    }
}