using System;
using System.Web;
using System.Web.Routing;
using Moq;
using ReturnNull.CanonicalRoutes.Models;

namespace ReturnNull.CanonicalRoutes.Tests.AssertHelpers
{
    internal class RequestDataBuilder
    {
        private RouteValueDictionary _values;
        private readonly Mock<HttpContextBase> _mockHttpContext;
        private readonly Mock<RouteBase> _route;
        private Uri _uri;
        private RouteBase _fixedRoute = null;

        public RequestDataBuilder()
        {
            _values = new RouteValueDictionary();
            _mockHttpContext = new Mock<HttpContextBase>();
            _route = new Mock<RouteBase>();
            _uri = new Uri("http://www.fail-fast.net/");
        }

        public static implicit operator RequestData(RequestDataBuilder builder)
        {
            return new RequestData(
                builder._mockHttpContext.Object, 
                builder._fixedRoute ?? builder._route.Object, 
                builder._values, 
                builder._uri);
        }

        public RequestDataBuilder WithValues(RouteValueDictionary routeValueDictionary)
        {
            _values = routeValueDictionary;
            return this;
        }

        public RequestDataBuilder WithUri(Uri uri)
        {
            _uri = uri;
            return this;
        }

        public RequestData WithRoute(RouteBase route)
        {
            _fixedRoute = route;
            return this;
        }
    }
}