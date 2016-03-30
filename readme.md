# Usage
1. Add the global action filter:

    ```
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //...
            filters.Add(new HandleDuplicateContentFilter());
            //...
        }
    }
    ```
2. Configure the SEO rules:

    ```
    // > /App_Data/CanonicalConfig.cs
    public class CanonicalConfig
    {
        public static void SetupCanonicalRules(CanonicalRulesetCollection rules)
        {
            rules.Add("Default", CanonicalRuleset.Recommended(new Uri("http://www.my-site.com")));
        }
    }
    
    // > /Global.asax.cs
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //...
            CanonicalConfig.SetupCanonicalRules(CanonicalRulesetCollection.Rules);
            //...
        }
    }
    ```
3. Define your canonical actions (usually GET actions):

    ```
    [Canonical(Ruleset = "Default", Query = new[] { "name" }, Sensitive = new[] { "id" })]
    public ActionResult About(string id, string name)
    {
        return View(new {id, name});
    }
    ```
