using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReturnNull.CanonicalRoutes.Models;
using ReturnNull.CanonicalRoutes.Rules.Abstract;

namespace ReturnNull.CanonicalRoutes.Rules
{
    public class EnforceHost : ICanonicalRule
    {
        private readonly string _host;
        private readonly int? _port;

        public EnforceHost(string host, int? port = null)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if(!Regex.IsMatch(host, @"^[a-z0-9\-]{1,253}(?:.[a-z0-9\-]{1,253})*$"))
                throw new ArgumentException("Please provide a valid hostname");
            _host = host;
            _port = port;
        }

        public bool HasBeenViolated(Uri url, RouteInfo routeInfo, UserProvisions provisions)
        {
            return url.Host == _host && (_port == null ? url.IsDefaultPort : _port == url.Port);
        }

        public void CorrectPlan(UrlPlan plan, RouteInfo routeInfo, UserProvisions provisions)
        {
            var builder = new UriBuilder(plan.Authority);

            builder.Host = _host;
            if (_port.HasValue)
                builder.Port = _port.Value;

            plan.Authority = builder.Uri.GetLeftPart(UriPartial.Authority);
        }
    }
}
