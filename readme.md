# Install
Use nuget: `Install-Package ReturnNull.Seo.DuplicateContent`

see: https://www.nuget.org/packages/ReturnNull.Seo.DuplicateContent/

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
    // > /App_Data/SeoConfig.cs
    public class SeoConfig
    {
        public static void SetupDuplicateContentRules(SeoRequestRulesetCollection rules)
        {
            rules.Add("Default", SeoRequestRuleset.Recommended());
        }
    }
    
    // > /Global.asax.cs
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //...
            CanonicalConfig.SetupCanonicalRules(SeoRequestRulesetCollection.Rules);
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
4. Add your canonical meta tag:
    ```
    <meta rel="canonical" href="@Url.Canonical()" />
    ```

# The canonical attribute.
The canonical attribute can be applied to any `Action` or `Controller`. If applied to both, then the `Ruleset` will be taken from the controller unless overridden on the attribute on the action. The Query and Sensitive arrays will be merged.

## Ruleset
The ruleset property is a string which should match one of the rulesets defined during Application_Start.

## Query
The query property is a list of keys that the action expects and that is very influencial on the content shown on the page. 
For example, if in the url `http://www.my-site.com/home/about?person=Jane` the resulting content shows information about the person Jane, then "person" is a canonical Query key and should be defined.

## Sensitive
The sensitive property applies to both route parameters as well as canonical querystring parameters (but not other querystrings). By default, we will assume any of the values provided to your application is case insensitive and can be lowercased for SEO purposes. If you have a case sensitive canonical querystring or route parameter, put the parameter name here.

An example of this might be:  
route: `"emoji/{emoji}"`  
url: `http://www.my-site.com/emoji/o_O`  
attribute: `[Canonical(Ruleset = "Default", Sensitive = new[] { "emoji" })]`   
-- or --  
route: `"search"`  
url: `http://www.my-site.com/search?q=User+inpuT`   
attribute: `[Canonical(Ruleset = "Default", Query = new[] { "q" }, Sensitive = new[] { "q" })]`   

#The recommended ruleset
The above example shows starting off with a recommended ruleset. The source of this ruleset can be found here: [SeoRequestRuleset.cs](ReturnNull.CanonicalRoutes/Configuration/SeoRequestRuleset.cs)

**The provided rules do not validate or normalize non-canonical querystring parameters.**

By default, this ruleset will use a 301 redirect to normalize urls if:

1. A canonical querystring has an uppercase letter in its key
2. A canonical, non-sensitive querystring has an uppercase letter in its value
3. The non-sensitive route parameters are not lowercased and the rest of the url matches the case in the route definition
4. The url matches the simplified version of the route (optional parameters should be ommitted unless it is different to the default)
5. There is a trailing slash in the url
6. There are repeating slashes in the url

If the request does not violate any of the rules defined in the RedirectRules collection, then there will be no redirect, but RewriteRules will be applied to the request to generate the canonical version of the requested url for your canonical meta tag:

1. Any querystrings not defined as canonical will be removed
2. All canonical querystrings will be ordered by key alphabetically

## Optional parameters
You can pass an optional parameter to the Recommended method in order to apply some extra rules. 
Providing a Uri will allow us to enforce the scheme and hostname of your site; this is useful if you have a preferred domain and scheme defined in your configuration file.

You can read more about the rules here: [Redirect Rules](rules.md)
