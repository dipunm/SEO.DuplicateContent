using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReturnNull.CanonicalRoutes.Mvc;

namespace Demo.Web.Controllers
{
    [Canonical(Ruleset = "Default")]
    public class HomeController : Controller
    {
        [Canonical(RouteName = "another")]
        public ActionResult Index()
        {
            return View();
        }

        [Canonical(Ruleset = "SSL")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Canonical(Query = new[] { "person", "id", "person.age" }, Sensitive = new [] { "id", "person" })]
        public ActionResult Contact(string id = null, string person = null)
        {
            ViewBag.Message = "Your contact page.";
            ViewBag.Person = id ?? person ?? "John";

            return View();
        }

        [Canonical(RouteName = "attribute")]
        [Route("attribute-url", Name = "attribute")]
        [Route("attribute-of-{id:int}")]
        public ActionResult Attribute()
        {
            return Content("This is the Attribute Routed Page!");
        }
    }
}