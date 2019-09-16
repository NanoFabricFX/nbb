using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class SubscriptionFeature
    {
        public Guid SubcriptionId { get; set; }
        public Guid FeatureId { get; set; }
        public decimal FeatureValue { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual Subscription Subcription { get; set; }
    }
}