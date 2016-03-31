using System.Collections.Generic;

namespace ReturnNull.CanonicalRoutes.Rules.Dependencies
{
    public interface ISlugProvider
    {
        string GetSlug(string slugId, IEnumerable<KeyValuePair<string, object>> requestData);
    }
}