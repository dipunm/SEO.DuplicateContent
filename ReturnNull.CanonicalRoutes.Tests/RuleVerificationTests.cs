using System;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules;
using ReturnNull.CanonicalRoutes.Rules.Abstract;
using ReturnNull.CanonicalRoutes.Rules.Dependencies;
using ReturnNull.CanonicalRoutes.Tests.AssertHelpers;
using Shouldly;

namespace ReturnNull.CanonicalRoutes.Tests
{
    [TestFixture]
    public class RuleVerificationTests
    {
        private UserProvisions NoProvisions { get; } = new UserProvisions(new string[0], new string[0]);

        private RequestDataBuilder CreateRequestData()
        {
            return new RequestDataBuilder();
        }

        [Test]
        public void EnforceCorrectSlug_GivenNullSlugProvider_ShouldThrowNullArgumentException()
        {
            new Action(() => { new EnforceCorrectSlug(null); }).ShouldThrow<ArgumentException>();
        }

        [Test]
        public void EnforceCorrectSlug_WhenProvidedRouteValueEndingWithSlug_ShouldCheckValueWithProvider()
        {
            var mockSlugProvider = new Mock<ISlugProvider>();
            var rule = new EnforceCorrectSlug(mockSlugProvider.Object);
            
            rule.HasBeenViolated(
                CreateRequestData().WithValues(new RouteValueDictionary() { {"myParamSlug", "this-is-my-value"} }), 
                NoProvisions);

            mockSlugProvider.Verify(p => p.GetSlug("myParamSlug"));
        }

        [Test]
        public void EnforceCorrectSlug_WhenProvidedRouteValueNotEndingWithSlug_ShouldNotCheckValueWithProvider()
        {
            var mockSlugProvider = new Mock<ISlugProvider>();
            var rule = new EnforceCorrectSlug(mockSlugProvider.Object);

            rule.HasBeenViolated(
                CreateRequestData().WithValues(new RouteValueDictionary() { { "myParam", "this-is-my-value" } }),
                NoProvisions);

            mockSlugProvider.Verify(p => p.GetSlug("myParam"), Times.Never);
        }

        [Test]
        public void EnforceCorrectSlug_WhenSlugFromProviderIsSameAsRouteValue_ShouldNotViolateRule()
        {
            var mockSlugProvider = new Mock<ISlugProvider>();
            var rule = new EnforceCorrectSlug(mockSlugProvider.Object);

            mockSlugProvider.Setup(p => p.GetSlug("myParamSlug"))
                .Returns("this-is-my-value");

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithValues(new RouteValueDictionary() { { "myParamSlug", "this-is-my-value" } }),
                NoProvisions);

            violated.ShouldBe(false);
        }

        [Test]
        public void EnforceCorrectSlug_WhenSlugFromProviderIsDifferentToRouteValue_ShouldViolateRule()
        {
            var mockSlugProvider = new Mock<ISlugProvider>();
            var rule = new EnforceCorrectSlug(mockSlugProvider.Object);

            mockSlugProvider.Setup(p => p.GetSlug("myParamSlug"))
                .Returns("this-is-my-value");

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithValues(new RouteValueDictionary() { { "myParamSlug", "THIS-IS-MY-VALUE" } }),
                NoProvisions);

            violated.ShouldBe(true);
        }

        [Test]
        public void EnforceCorrectSlug_WhenNoSlugParametersToCheck_ShouldNotViolateRule()
        {
            var mockSlugProvider = new Mock<ISlugProvider>();
            var rule = new EnforceCorrectSlug(mockSlugProvider.Object);

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithValues(new RouteValueDictionary() { { "myParam", "THIS-IS-MY-VALUE" } }),
                NoProvisions);

            violated.ShouldBe(false);
        }

        [TestCase("www.")]
        [TestCase("www/asd")]
        [TestCase("LOCALHOST")]
        public void EnforceHost_GivenInvalidHost_ShouldThrowArgumentException(string host)
        {
            new Action(() => new EnforceHost(host)).ShouldThrow<ArgumentException>();
        }

        [TestCase("www.mysite.com")]
        [TestCase("www.my-site.co.uk")]
        [TestCase("192.168.0.1")]
        [TestCase("localhost")]
        public void EnforceHost_GivenValidHost_ShouldNotThrowArgumentException(string host)
        {
            new Action(() => new EnforceHost(host)).ShouldNotThrow();
        }
        
        [Test]
        public void EnforceHost_GivenNegativePort_ShouldThrowArgumentException()
        {
            new Action(() => new EnforceHost("www.my-site.com", -100)).ShouldThrow<ArgumentException>();
        }

        [Test]
        public void EnforceHost_WhenHostDoesNotMatch_ShouldViolateRule()
        {
            var rule = new EnforceHost("www.my-site.com");

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/path/to/page")),
                NoProvisions);

            violated.ShouldBe(true);
        }

        [Test]
        public void EnforceHost_WhenHostMatches_ShouldNotViolateRule()
        {
            var rule = new EnforceHost("www.my-site.com");

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://www.my-site.com/path/to/page")),
                NoProvisions);

            violated.ShouldBe(false);
        }

        [Test]
        public void EnforceHost_WhenHttpPortIsNot80_ShouldViolateRule()
        {
            var rule = new EnforceHost("www.my-site.com");

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://www.my-site.com:443/path/to/page")),
                NoProvisions);

            violated.ShouldBe(true);
        }

        [Test]
        public void EnforceHost_WhenHttpsPortIsNot443_ShouldViolateRule()
        {
            var rule = new EnforceHost("www.my-site.com");

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("https://www.my-site.com:80/path/to/page")),
                NoProvisions);

            violated.ShouldBe(true);
        }

        [Test]
        public void EnforceHost_WhenPortExpected_WhenMatchingPort_ShouldNotViolateRule()
        {
            var rule = new EnforceHost("www.my-site.com", 1234);

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://www.my-site.com:1234/path/to/page")),
                NoProvisions);

            violated.ShouldBe(false);
        }

        [Test]
        public void EnforceHost_WhenPortExpected_WhenUsingDefaultPort_ShouldViolateRule()
        {
            var rule = new EnforceHost("www.my-site.com", 1234);

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://www.my-site.com/path/to/page")),
                NoProvisions);

            violated.ShouldBe(true);
        }

        [TestCase("http")]
        [TestCase("https")]
        public void EnforceScheme_GivenMatchingScheme_ShouldNotViolateRule(string scheme)
        {
            var rule = new EnforceScheme(scheme);

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri($"{scheme}://www.my-site.com/path/to/page")),
                NoProvisions);

            violated.ShouldBe(false);
        }

        [Test]
        public void EnforceScheme_GivenNonMatchingScheme_ShouldViolateRule()
        {
            var rule = new EnforceScheme(EnforceScheme.Http);

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("https://www.my-site.com/path/to/page")),
                NoProvisions);

            violated.ShouldBe(true);
        }

        [Test]
        public void LowercaseQuerystringKeys_GivenUppercaseLetterInCanonicalQueryKey_ShouldViolateRule()
        {
            var rule = new LowercaseQuerystringKeys();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?Key=value")), 
                new UserProvisions(new string[0], new [] {"key"}));

            violated.ShouldBe(true);
        }

        [Test]
        public void LowercaseQuerystringKeys_GivenNoCanonicalQueryKeys_ShouldNotViolateRule()
        {
            var rule = new LowercaseQuerystringKeys();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?Key=value")),
                NoProvisions);

            violated.ShouldBe(false);
        }

        [Test]
        public void LowercaseQuerystringKeys_GivenUppercaseLetterInNonCanonicalQueryKey_ShouldNotViolateRule()
        {
            var rule = new LowercaseQuerystringKeys();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?key=value&TRACK=2020")),
                new UserProvisions(new string[0], new[] { "key" }));


            violated.ShouldBe(false);
        }

        [Test]
        public void LowercaseQuerystringValues()
        {
            Assert.Fail();
        }
    }
}
