using System.Collections.Generic;

namespace ReturnNull.CanonicalRoutes.Mvc
{
    public interface ICanonicalConstraintResolver
    {
        /// <summary>
        /// Constraints that should be applied to routes that are not given
        /// a prefered canonical Route.
        /// 
        /// When executing an action through these routes, the route is 
        /// considered 'Neutral' because no preferred route has been set on 
        /// this action for SEO purposes via the Canonical attribute.
        /// </summary>
        IReadOnlyDictionary<string, object> NeutralRouteConstraints { get; }

        /// <summary>
        /// Constraints that should be applied to routes when matching against
        /// actions that have this route set as the canonical route.
        /// 
        /// When executing an action through these routes, the route is 
        /// considered 'Canonical' because it has been identified as the 
        /// preferred route for SEO purposes via the Canonical attribute.
        /// </summary>
        IReadOnlyDictionary<string, object> CanonicalRouteConstraints { get; }

        /// <summary>
        /// Constraints that should be applied to routes when matching against
        /// actions that have a different route set as the canonical route.
        /// 
        /// When executing an action through these routes, the route is 
        /// considered 'Legacy' because another route has been identified as 
        /// the preferred route for SEO purposes via the Canonical attribute.
        /// </summary>
        IReadOnlyDictionary<string, object> LegacyRouteConstraints { get; }
    }

    public class CanonicalConstraintResolver : ICanonicalConstraintResolver
    {
        public CanonicalConstraintResolver()
        {
            NeutralRouteConstraints = new Dictionary<string, object>();
            CanonicalRouteConstraints = new Dictionary<string, object>();
            LegacyRouteConstraints = new Dictionary<string, object>();
        }

        /// <summary>
        /// Constraints that should be applied to routes that are not given
        /// a prefered canonical Route.
        /// 
        /// When executing an action through these routes, the route is 
        /// considered 'Neutral' because no preferred route has been set on 
        /// this action for SEO purposes via the Canonical attribute.
        /// </summary>
        public Dictionary<string, object> NeutralRouteConstraints { get; }

        /// <summary>
        /// Constraints that should be applied to routes when matching against
        /// actions that have this route set as the canonical route.
        /// 
        /// When executing an action through these routes, the route is 
        /// considered 'Canonical' because it has been identified as the 
        /// preferred route for SEO purposes via the Canonical attribute.
        /// </summary>
        public Dictionary<string, object> CanonicalRouteConstraints { get; }

        /// <summary>
        /// Constraints that should be applied to routes when matching against
        /// actions that have a different route set as the canonical route.
        /// 
        /// When executing an action through these routes, the route is 
        /// considered 'Legacy' because another route has been identified as 
        /// the preferred route for SEO purposes via the Canonical attribute.
        /// </summary>
        public Dictionary<string, object> LegacyRouteConstraints { get; }

        IReadOnlyDictionary<string, object> ICanonicalConstraintResolver.NeutralRouteConstraints
            => NeutralRouteConstraints;

        IReadOnlyDictionary<string, object> ICanonicalConstraintResolver.CanonicalRouteConstraints
            => CanonicalRouteConstraints;

        IReadOnlyDictionary<string, object> ICanonicalConstraintResolver.LegacyRouteConstraints
            => LegacyRouteConstraints;
    }
}