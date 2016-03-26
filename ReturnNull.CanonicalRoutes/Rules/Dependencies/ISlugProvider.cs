namespace ReturnNull.CanonicalRoutes.Rules.Dependencies
{
    public interface ISlugProvider
    {
        string GetSlug(string key);
    }
}