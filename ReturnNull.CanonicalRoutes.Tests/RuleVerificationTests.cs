using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
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
        private UserProvisions NoProvisions { get; } = new UserProvisions(new string[0], new string[0], null);

        private RequestDataBuilder CreateRequestData()
        {
            return new RequestDataBuilder();
        }

        [SetUp]
        public void Setup()
        {
            RouteTable.Routes.Clear();
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
                CreateRequestData().WithValues(new RouteValueDictionary() { { "myParamSlug", "this-is-my-value" } }),
                NoProvisions);

            mockSlugProvider.Verify(p => p.GetSlug("myParamSlug", It.IsAny<IEnumerable<KeyValuePair<string, object>>>()));
        }

        [Test]
        public void EnforceCorrectSlug_WhenProvidedRouteValueNotEndingWithSlug_ShouldNotCheckValueWithProvider()
        {
            var mockSlugProvider = new Mock<ISlugProvider>();
            var rule = new EnforceCorrectSlug(mockSlugProvider.Object);

            rule.HasBeenViolated(
                CreateRequestData().WithValues(new RouteValueDictionary() { { "myParam", "this-is-my-value" } }),
                NoProvisions);

            mockSlugProvider.Verify(p => p.GetSlug("myParam", It.IsAny<IEnumerable<KeyValuePair<string, object>>>()), Times.Never);
        }

        [Test]
        public void EnforceCorrectSlug_WhenSlugFromProviderIsSameAsRouteValue_ShouldNotViolateRule()
        {
            var mockSlugProvider = new Mock<ISlugProvider>();
            var rule = new EnforceCorrectSlug(mockSlugProvider.Object);

            mockSlugProvider.Setup(p => p.GetSlug("myParamSlug", It.IsAny<IEnumerable<KeyValuePair<string, object>>>()))
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

            mockSlugProvider.Setup(p => p.GetSlug("myParamSlug", It.IsAny<IEnumerable<KeyValuePair<string, object>>>()))
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
                new UserProvisions(new string[0], new[] { "key" }, null));

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
                new UserProvisions(new string[0], new[] { "key" }, null));


            violated.ShouldBe(false);
        }

        [Test]
        public void LowercaseQuerystringValues_GivenUppercaseLetterInNonCanonicalQueryValue_ShouldNotViolateRule()
        {
            var rule = new LowercaseQuerystringValues();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?key=value&track=COMPANY-A")),
                new UserProvisions(new string[0], new[] { "key" }, null));


            violated.ShouldBe(false);
        }

        [Test]
        public void LowercaseQuerystringValues_GivenUppercaseLetterInCanonicalQueryValue_ShouldViolateRule()
        {
            var rule = new LowercaseQuerystringValues();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?key=Value")),
                new UserProvisions(new string[0], new[] { "key" }, null));

            violated.ShouldBe(true);
        }

        [Test]
        public void LowercaseQuerystringValues_GivenNoCanonicalQueryValues_ShouldNotViolateRule()
        {
            var rule = new LowercaseQuerystringValues();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?key=Value")),
                NoProvisions);

            violated.ShouldBe(false);
        }

        [Test]
        public void LowercaseQuerystringValues_GivenUppercaseLetterInSensitiveCanonicalQueryValue_ShouldNotViolateRule()
        {
            var rule = new LowercaseQuerystringValues();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?key=Value")),
                new UserProvisions(new[] { "key" }, new[] { "key" }, null));

            violated.ShouldBe(false);
        }

        [Test]
        public void OrderQuerystrings_GivenQuerystringsInOrder_ShouldNotViolateRule()
        {
            var rule = new OrderQuerystrings();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?queryA=0&queryB=0&queryc=0&queryd=0")),
                new UserProvisions(new string[0], new[] { "querya", "queryb", "queryc", "queryd" }, null));

            violated.ShouldBe(false);
        }

        [Test]
        public void OrderQuerystrings_GivenQuerystringsNotInOrder_ShouldViolateRule()
        {
            var rule = new OrderQuerystrings();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?queryB=0&queryc=0&queryd=0&queryA=0")),
                new UserProvisions(new string[0], new[] { "querya", "queryb", "queryc", "queryd" }, null));

            violated.ShouldBe(true);
        }

        [Test]
        public void OrderQuerystrings_GivenQuerystringsNotInOrderButNotCanonical_ShouldNotViolateRule()
        {
            var rule = new OrderQuerystrings();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?queryB=0&queryc=0&queryd=0&queryA=0")),
                new UserProvisions(new string[0], new[] { "queryb" }, null));

            violated.ShouldBe(false);
        }

        [Test]
        public void OrderQuerystrings_GivenCanonicalQuerystringsAtBeginning_ShouldViolateRule()
        {
            var rule = new OrderQuerystrings();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?queryc=0&queryB=0&queryd=0&queryA=0")),
                new UserProvisions(new string[0], new[] { "queryb" }, null));

            violated.ShouldBe(true);
        }

        [TestCase("http://my-site.com//")]
        [TestCase("http://my-site.com/hello//world")]
        [TestCase("http://my-site.com/hello/world//")]
        [TestCase("http://my-site.com/hello/world//")]
        [TestCase("http://my-site.com/hello////world/")]
        public void RemoveRepeatingSlashes_GivenMultipleSlashes_ShouldViolateRule(string url)
        {
            var rule = new RemoveRepeatingSlashes();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri(url)),
                NoProvisions);

            violated.ShouldBe(true);
        }

        [Test]
        public void RemoveRepeatingSlashes_GivenSingleSlashes_ShouldNotViolateRule()
        {
            var rule = new RemoveRepeatingSlashes();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/webpath/webfile")),
                NoProvisions);

            violated.ShouldBe(false);
        }

        [Test]
        public void RemoveTrailingSlash_GivenTrailingSlash_ShouldViolateRule()
        {
            var rule = new RemoveTrailingSlash();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/webpath/webfile/")),
                NoProvisions);

            violated.ShouldBe(true);
        }

        [Test]
        public void RemoveTrailingSlash_GivenNoTrailingSlash_ShouldNotViolateRule()
        {
            var rule = new RemoveTrailingSlash();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/webpath/webfile")),
                NoProvisions);

            violated.ShouldBe(false);
        }

        [Test]
        public void RemoveTrailingSlash_GivenDomainUrl_ShouldNotViolateRule()
        {
            var rule = new RemoveTrailingSlash();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/")),
                NoProvisions);

            violated.ShouldBe(false);
        }

        [Test]
        public void RemoveUncanonicalQuerystrings_GivenOnlyCanonicalQuerystrings_ShouldNotViolateRule()
        {
            var rule = new RemoveUncanonicalQuerystrings();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?canonical=true&canonical2=100")),
                new UserProvisions(new string[0], new[] { "canonical", "canonical2" }, null));

            violated.ShouldBe(false);
        }

        [Test]
        public void RemoveUncanonicalQuerystrings_GivenOnlyNonCanonicalQuerystrings_ShouldViolateRule()
        {
            var rule = new RemoveUncanonicalQuerystrings();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?notcanonical=true&notcanonical2=100")),
                new UserProvisions(new string[0], new[] { "canonical", "canonical2" }, null));

            violated.ShouldBe(true);
        }

        [Test]
        public void RemoveUncanonicalQuerystrings_GivenMixOfCanonicalAndNonCanonicalQuerystrings_ShouldViolateRule()
        {
            var rule = new RemoveUncanonicalQuerystrings();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/?notcanonical=true&canonical2=100")),
                new UserProvisions(new string[0], new[] { "canonical", "canonical2" }, null));

            violated.ShouldBe(true);
        }

        [Test]
        public void RemoveUncanonicalQuerystrings_GivenNoQuerystrings_ShouldNotViolateRule()
        {
            var rule = new RemoveUncanonicalQuerystrings();

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithUri(new Uri("http://my-site.com/")),
                new UserProvisions(new string[0], new[] { "canonical", "canonical2" }, null));

            violated.ShouldBe(false);
        }

        [Test]
        public void EnforceCorrectRoute_GivenNoRouteNameInUserPreferences_ShouldNotViolateRule()
        {
            var rule = new EnforceCorrectRoute();
            
            var violated = rule.HasBeenViolated(
                CreateRequestData(),
                new UserProvisions(new string[0], new string [0], null));

            violated.ShouldBe(false);
        }

        [Test]
        public void EnforceCorrectRoute_GivenMatchingRoute_ShouldNotViolateRule()
        {
            var rule = new EnforceCorrectRoute();

            var route1 = new Mock<RouteBase>().Object;
            RouteTable.Routes.Add("route1", route1);
            var violated = rule.HasBeenViolated(
                CreateRequestData().WithRoute(route1),
                new UserProvisions(new string[0], new string[0], "route1"));

            violated.ShouldBe(false);
        }
        
        [Test]
        public void EnforceCorrectRoute_GivenMismatchingRoute_ShouldViolateRule()
        {
            var rule = new EnforceCorrectRoute();

            var route1 = new Mock<RouteBase>().Object;
            RouteTable.Routes.Add("route1", route1);

            var route2 = new Mock<RouteBase>().Object;
            RouteTable.Routes.Add("route2", route2);

            var violated = rule.HasBeenViolated(
                CreateRequestData().WithRoute(route1),
                new UserProvisions(new string[0], new string[0], "route2"));

            violated.ShouldBe(true);
        }
        
        /// <summary>
        /// Attribute Routing creates 2 routes, so matching routes by name is not possible.
        /// There is also only an internal link between the two routes.
        /// </summary>
        [Test]
        public void EnforceCorrectRoute_WhenNamedRouteIsFromAttributeRouting_ShouldVerifyBy()
        {
            var rule = new EnforceCorrectRoute();
            var routeMock = new Mock<RouteBase>();
            routeMock.Setup(m => m.GetVirtualPath(It.IsAny<RequestContext>(), It.IsAny<RouteValueDictionary>()))
                .Returns(new VirtualPathData(routeMock.Object, "/custom-url?e=extra&p=parameters"));
            var routeInternal = routeMock.Object;
            SimulateAttributeRoutingWithOneRoute(RouteTable.Routes, routeInternal, routeName: "route1");
            
            var violated = rule.HasBeenViolated(
                //With Attribute routing, when getting the current route from the request, 
                //it returns a reference to the original Route instead of a wrapper.
                CreateRequestData().WithRoute(routeInternal),
                new UserProvisions(new string[0], new string[0], "route1"));
            
            violated.ShouldBe(false);
        }

        private void SimulateAttributeRoutingWithOneRoute(RouteCollection routes, RouteBase route, string routeName = null)
        {
            var collectionRoute = new Mock<RouteBase>(); //would be a collection containing {route}
            collectionRoute.Setup(r => r.GetRouteData(It.IsAny<HttpContextBase>()))
                .Returns(new RouteData(route, null));

            var linkGenerationRoute = new Mock<RouteBase>(); //would be a collection containing {route}
            linkGenerationRoute.Setup(r => r.GetVirtualPath(It.IsAny<RequestContext>(), It.IsAny<RouteValueDictionary>()))
                .Returns<RequestContext, RouteValueDictionary>(route.GetVirtualPath); //this route just delegates to the original route.

            routes.Add(collectionRoute.Object); //used for httpContext -> route
            routes.Add(routeName, linkGenerationRoute.Object); //used for Url.Action -> virtualPath
        }
    }
}
