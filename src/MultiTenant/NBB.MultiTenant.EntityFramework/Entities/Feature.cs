using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class Feature
    {
        public Feature()
        {
            FeatureUserRights = new HashSet<FeatureUserRight>();
            SubscriptionFeatures = new HashSet<SubscriptionFeature>();
            TenantFeatures = new HashSet<TenantFeature>();
            UserFeatures = new HashSet<UserFeature>();
        }

        public Guid FeatureId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<FeatureUserRight> FeatureUserRights { get; set; }
        public virtual ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; }
        public virtual ICollection<TenantFeature> TenantFeatures { get; set; }
        public virtual ICollection<UserFeature> UserFeatures { get; set; }
    }
}