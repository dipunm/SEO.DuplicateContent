using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using ReturnNull.CanonicalRoutes.Mvc;

namespace Routes.Controllers
{
    [Canonical(Ruleset = "Default")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Canonical(Query = new[] { "a", "b", "c" })]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /*
        [Route("brands/{brandSlug}/{brandId}")]
        [Canonicalize(Rule = "Default", Sensitive = new []{"brandId"}, Query = new [] {"a","b","c"})]
        public ActionResult Brands(string brandId)
        {
            Canonical.Slug("brandSlug", "this-is-the-slug");
            return View("Index");
        }
        */
    }

}