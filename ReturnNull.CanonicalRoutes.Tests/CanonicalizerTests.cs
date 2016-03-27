using System;
using System.Collections.Specialized;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using ReturnNull.CanonicalRoutes.Configuration;
using ReturnNull.CanonicalRoutes.Internal;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;
using ReturnNull.CanonicalRoutes.Tests.AssertHelpers;
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
        
        CanonicalRuleset.Recommended -- test for parameters.
        UrlPlan.Execute? -- probably not.
        */


    [TestFixture]
    public class CanonicalizerTests
    {
        private CanonicalRuleset _rules;
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

            _rules = new CanonicalRuleset();
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
