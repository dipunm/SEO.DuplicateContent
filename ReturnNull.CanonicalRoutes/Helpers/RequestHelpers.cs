using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ReturnNull.CanonicalRoutes.Helpers
{
    internal static class RequestHelpers
    {
        internal static Uri GetRealRequestUri(this HttpRequestBase request)
        {
            return new UriBuilder(
                $"{request.Url?.Scheme}://" +
                $"{request.ServerVariables["HTTP_HOST"]}" +
                $"{request.ServerVariables["HTTP_URL"]}" +
                $"{request.ServerVariables["HTTP_QUERY"]}").Uri;
        }
    }
}
