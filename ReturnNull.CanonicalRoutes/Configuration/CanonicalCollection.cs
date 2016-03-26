using System;
using System.Collections.Generic;

namespace ReturnNull.CanonicalRoutes.Configuration
{
    public class CanonicalCollection
    {
        private readonly Dictionary<string, CanonicalRuleSet> _ruleSets;
        private CanonicalCollection()
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
            return _ruleSets[name];
        }

        public static CanonicalCollection Rules { get; } = new CanonicalCollection();
    }
}