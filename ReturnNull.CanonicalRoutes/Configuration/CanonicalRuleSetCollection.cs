using System;
using System.Collections.Generic;

namespace ReturnNull.CanonicalRoutes.Configuration
{
    public class CanonicalRuleSetCollection
    {
        private readonly Dictionary<string, CanonicalRuleSet> _ruleSets;
        private CanonicalRuleSetCollection()
        {
            _ruleSets = new Dictionary<string, CanonicalRuleSet>();
        }

        public void Add(string name, CanonicalRuleSet ruleSet)
        {
            if(_ruleSets.ContainsKey(name))
                throw new ArgumentException($"RuleSet with name '{name}' already exists.", nameof(name));

            _ruleSets.Add(name, ruleSet);
        }

        public CanonicalRuleSet Get(string name)
        {
            if(_ruleSets.ContainsKey(name))
                return _ruleSets[name];

            throw new ArgumentException($"No rulesets found by the name '{name}'", nameof(name));
        }

        public static CanonicalRuleSetCollection Rules { get; } = new CanonicalRuleSetCollection();
    }
}