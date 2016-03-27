using System;
using System.Collections.Generic;

namespace ReturnNull.CanonicalRoutes.Configuration
{
    public class CanonicalRulesetCollection
    {
        private readonly Dictionary<string, CanonicalRuleset> _rulesets;
        private CanonicalRulesetCollection()
        {
            _rulesets = new Dictionary<string, CanonicalRuleset>();
        }

        public void Add(string name, CanonicalRuleset ruleset)
        {
            if(_rulesets.ContainsKey(name))
                throw new ArgumentException($"Ruleset with name '{name}' already exists.", nameof(name));

            _rulesets.Add(name, ruleset);
        }

        public CanonicalRuleset Get(string name)
        {
            if(_rulesets.ContainsKey(name))
                return _rulesets[name];

            throw new ArgumentException($"No rulesets found by the name '{name}'", nameof(name));
        }

        public static CanonicalRulesetCollection Rules { get; } = new CanonicalRulesetCollection();
    }
}