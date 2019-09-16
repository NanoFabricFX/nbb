using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class Subscription
    {
        public Subscription()
        {
            SubscriptionFeatures = new HashSet<SubscriptionFeature>();
            TenantSubcriptions = new HashSet<TenantSubcription>();
        }

        public Guid SubscriptionId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; }
        public virtual ICollection<TenantSubcription> TenantSubcriptions { get; set; }
    }
}