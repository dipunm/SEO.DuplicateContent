# Seo Request Rules
The ruleset system was designed to be flexible enough to allow new custom rules to be created for extensibility.

To create a custom ruleset, you may extend either `ISeoRequestRule` or `SeoRequestRuleSnitch`.

# ISeoRequestRule
Any rule created may be used to trigger a redirect, or to help rewrite the request url for the canonical meta tag. The usage will depend on how the rulesets are configured.
This interface contains two methods, one will be used to help determine if any action is required, and the other will process the request data.

## `HasBeenViolated(...)`
This method receives two parameters: requestData and provisions. RequestData contains the request Uri, the route values, and the matched route; there is also the HttpContext which may be necessary when interacting with the Route object.
Provisions contains the Query and Sensitive parameters that are configured on the Controller or Action. Use this to limit your validation logic to only relevant route values and querystrings where appropriate.

## `CorrectPlan(...)`
This method allows you to mutate a UrlPlan object to influence how the corrected url will look. The UrlPlan contains two string values: Authority and Query.
These values can be modified as you see fit, but you should be careful only to apply your rule and not indirectly apply another where possible. For example, a rule may re-order the canonical querystrings, but should not accidentally remove the non-canonical querystrings in the process.
The path of the corrected url is represented simply as route values. The path will be generated using the Route object and the mutated route values. You may add, remove or modify route values as you see appropriate, it is safe to do so and will not affect the rest of your MVC application.

# SeoRequestRuleSnitch
In some cases, you may have a rule that could cause a redirect, but the problem resides in the path of the url. If you are confident that the matched route is likely to automatically resolve the problem without changing the route values, you may implement this abstract class which allows you to only implement the HasBeenViolated method.