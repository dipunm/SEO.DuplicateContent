using System;
using System.Collections.Generic;

namespace ReturnNull.CanonicalRoutes.Configuration
{
    public class SeoRequestRulesetCollection
    {
        private readonly Dictionary<string, SeoRequestRuleset> _rulesets;
        private SeoRequestRulesetCollection()
        {
            _rulesets = new Dictionary<string, SeoRequestRuleset>();
        }

        public void Add(string name, SeoRequestRuleset ruleset)
        {
            if(_rulesets.ContainsKey(name))
                throw new ArgumentException($"Ruleset with name '{name}' already exists.", nameof(name));

            _rulesets.Add(name, ruleset);
        }

        public SeoRequestRuleset Get(string name)
        {
            if(_rulesets.ContainsKey(name))
                return _rulesets[name];

            throw new ArgumentException($"No rulesets found by the name '{name}'", nameof(name));
        }

        public static SeoRequestRulesetCollection Rules { get; } = new SeoRequestRulesetCollection();
    }
}