using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using ReturnNull.CanonicalRoutes.Configuration;
using ReturnNull.CanonicalRoutes.Helpers;
using ReturnNull.CanonicalRoutes.Internal;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;
using Shouldly;

namespace ReturnNull.CanonicalRoutes.Tests
{
        /**

        Test each Rule:
            - Remove Uncanonical Querystrings
            - Remove Trailing Slash
            - Remove Repeating Slashes
            - Order Querystrings
            - Omit Default Route Values
            - Match Case With Route
            - Lowercase Querystring Values
            - Lowercase Querystring Keys
            - Enforce Scheme
            - Enforce Host
            - Enforce Correct Slug

        Test ActionFilter
            - Finds attribute on action
            - Finds attribute on controller
            - Finds attribute on base controller
            - Merges values properly
            
            - Behaves when no canonicalizer found -- probably should inform why it failed, but don't fail silently
            - Responds to canonicalizer response properly.

        Test Canonicalizer
            - 
        Test QuerystringHelper
        
        Test CanonicalCollection
        
        CanonicalRuleSet.Recommended -- test for parameters.
        UrlPlan.Execute? -- probably not.
        */


    [TestFixture]
    public class QuerystringHelperTests
    {
        [Test]
        public void ToKeyValuePairs_GivenAnEmptyString_ShouldReturnEmpty()
        {
            "".ToKeyValuePairs().ShouldBeEmpty();
        }

        [Test]
        public void ToKeyValuePairs_StartingWithQuestionMark_ShouldExcludeFromResults()
        {
            "?key1=value1&key2=value2".ToKeyValuePairs().ShouldAllBe(pair => !pair.Key.Contains("?"));
        }

        [Test]
        public void ToKeyValuePairs_GivenNParameters_ShouldReturnNPairs()
        {
            const int N = 4;
            "key=value&key2=val&key3=value&key4=30".ToKeyValuePairs().Count().ShouldBe(N);
        }

        [Test]
        public void ToKeyValuePairs_WhenNoEquals_ShouldCreateNullValue()
        {
            "key".ToKeyValuePairs().Single().Value.ShouldBeNull();
        }

        [Test]
        public void ToKeyValuePairs_WhenNoEquals_ShouldTreatStringAsKey()
        {
            const string key = "key";
            key.ToKeyValuePairs().Single().Key.ShouldBe(key);
        }

        [Test]
        public void ToKeyValuePairs_WhenMultipleSubsequentAmpersands_ShouldTreatAsOneAmpersand()
        {
            "key=value&&key2=value2".ToKeyValuePairs().Count().ShouldBe(2);
        }

        [Test]
        public void ToKeyValuePairs_WhenDuplicateKeys_ShouldHaveAllKeyValuePairs()
        {
            "key=value&key=value2".ToKeyValuePairs().Count().ShouldBe(2);
        }

        [Test]
        public void ToKeyValuePairs_WhenMultipleEquals_ShouldTreatAnythingAfterFirstEqualAsValue()
        {
            "key=v=a=lue".ToKeyValuePairs().Single().Value.ShouldBe("v=a=lue");
        }

        [Test]
        public void ToQuerystring_WhenMultiplePairsProvided_ShouldJoinWithAmpersand()
        {
            new[]
            {
                new KeyValuePair<string, string>("key", "value"),
                new KeyValuePair<string, string>("key", "value2")
            }.ToQuerystring().ShouldBe("key=value&key=value2");
        }

        [Test]
        public void ToQuerystring_WhenPairsProvided_ShouldJoinKeyAndValueWithEqual()
        {
            new [] { new KeyValuePair<string, string>("key", "value")}.ToQuerystring().ShouldBe("key=value");
        }

        [Test]
        public void ToQuerystring_WhenPairValueIsNull_ShouldNotIncludeEqualCharacter()
        {
            new[] { new KeyValuePair<string, string>("key", null) }.ToQuerystring().ShouldBe("key");
        }
    }


    [TestFixture]
    public class CanonicalizerTests
    {
        private CanonicalRuleSet _rules;
        private Canonicalizer _canonicalizer;
        private RequestData _requestData;
        private UserProvisions NoProvisions { get; } = new UserProvisions(new string[0], new string[0]);

        [SetUp]
        public void Setup()
        {
            _requestData = new RequestData(
                new Mock<HttpContextBase>().Object, 
                new Mock<RouteBase>().Object, 
                new RouteValueDictionary(), 
                new Uri("http://www.fail-fast.net/"));

            _rules = new CanonicalRuleSet();
            _canonicalizer = new Canonicalizer(_rules);

        }

        [Test]
        public void Canonicalize_WhenThereAreNoRules_ShouldNotSuggestARedirect()
        {
            _rules.RedirectRules.Clear();
            _rules.RewriteRules.Clear();
            
            var results = _canonicalizer.Canonicalize(_requestData, NoProvisions);

            results.ShouldRedirect.ShouldBe(false);
        }

        [Test]
        public void Canonicalize_WhenThereAreNoRules_ShouldCreateDefaultPlan()
        {
            _rules.RedirectRules.Clear();
            _rules.RewriteRules.Clear();
            var defaultPlan = new UrlPlan(_requestData);
            
            var results = _canonicalizer.Canonicalize(_requestData, NoProvisions);

            results.Plan.ShouldMatch(defaultPlan);
        }

        [Test]
        public void Canonicalize_WhenRulesCorrectThePlan_ShouldReturnCorrectedPlan()
        {
            UrlPlan correctedPlan = null;
            var mockRewriteRule = new Mock<ICanonicalRule>();
            _rules.RewriteRules.Add(mockRewriteRule.Object);
            mockRewriteRule.Setup(r => r.CorrectPlan(It.IsAny<UrlPlan>(), It.IsAny<RequestData>(), It.IsAny<UserProvisions>()))
                .Callback<UrlPlan, RequestData, UserProvisions>((plan, requestData, provisions) => 
                correctedPlan = plan);

            var results = _canonicalizer.Canonicalize(_requestData, NoProvisions);

            results.Plan.ShouldBeSameAs(correctedPlan);
        }

        [Test]
        public void Canonicalize_WhenARedirectRuleHasNotBeenViolated_ShouldNotRunRedirectCorrections()
        {
            var mockRedirectRule = new Mock<ICanonicalRule>();
            _rules.RedirectRules.Add(mockRedirectRule.Object);
            mockRedirectRule.Setup(r => r.HasBeenViolated(_requestData, It.IsAny<UserProvisions>()))
                .Returns(false);

            _canonicalizer.Canonicalize(_requestData, NoProvisions);
            
            mockRedirectRule.Verify(
                r => r.CorrectPlan(It.IsAny<UrlPlan>(), It.IsAny<RequestData>(), It.IsAny<UserProvisions>()), 
                Times.Never);
        }

        [Test]
        public void Canonicalize_WhenARedirectRuleHasBeenViolated_ShouldNotRunRewriteCorrections()
        {
            var mockRedirectRule = new Mock<ICanonicalRule>();
            _rules.RedirectRules.Add(mockRedirectRule.Object);
            var mockRewriteRule = new Mock<ICanonicalRule>();
            _rules.RewriteRules.Add(mockRewriteRule.Object);
            mockRedirectRule.Setup(r => r.HasBeenViolated(_requestData, It.IsAny<UserProvisions>()))
                .Returns(true);

            _canonicalizer.Canonicalize(_requestData, NoProvisions);

            mockRewriteRule.Verify(
                r => r.CorrectPlan(It.IsAny<UrlPlan>(), It.IsAny<RequestData>(), It.IsAny<UserProvisions>()),
                Times.Never);
        }

        [Test]
        public void Canonicalize_WhenARedirectRuleHasNotBeenViolated_ShouldRunRewriteCorrections()
        {
            var mockRedirectRule = new Mock<ICanonicalRule>();
            _rules.RedirectRules.Add(mockRedirectRule.Object);
            var mockRewriteRule = new Mock<ICanonicalRule>();
            _rules.RewriteRules.Add(mockRewriteRule.Object);
            mockRedirectRule.Setup(r => r.HasBeenViolated(_requestData, It.IsAny<UserProvisions>()))
                .Returns(false);

            _canonicalizer.Canonicalize(_requestData, NoProvisions);

            mockRewriteRule.Verify(
                r => r.CorrectPlan(It.IsAny<UrlPlan>(), It.IsAny<RequestData>(), It.IsAny<UserProvisions>()),
                Times.Once);
        }

        [Test]
        public void Canonicalize_WhenARedirectRuleHasBeenViolated_ShouldRunRedirectCorrections()
        {
            var mockRedirectRule = new Mock<ICanonicalRule>();
            _rules.RedirectRules.Add(mockRedirectRule.Object);
            var mockRewriteRule = new Mock<ICanonicalRule>();
            _rules.RewriteRules.Add(mockRewriteRule.Object);
            mockRedirectRule.Setup(r => r.HasBeenViolated(_requestData, It.IsAny<UserProvisions>()))
                .Returns(true);

            _canonicalizer.Canonicalize(_requestData, NoProvisions);

            mockRedirectRule.Verify(
                r => r.CorrectPlan(It.IsAny<UrlPlan>(), It.IsAny<RequestData>(), It.IsAny<UserProvisions>()),
                Times.Once);
        }

        [Test]
        public void Canonicalize_WhenARedirectRuleHasBeenViolated_ShouldSuggestARedirect()
        {
            var mockRedirectRule = new Mock<ICanonicalRule>();
            _rules.RedirectRules.Add(mockRedirectRule.Object);
            mockRedirectRule.Setup(r => r.HasBeenViolated(_requestData, It.IsAny<UserProvisions>()))
                .Returns(true);

            var results = _canonicalizer.Canonicalize(_requestData, NoProvisions);

            results.ShouldRedirect.ShouldBe(true);
        }

    }
}
