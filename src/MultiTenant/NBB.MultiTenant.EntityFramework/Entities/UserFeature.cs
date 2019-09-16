using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class UserFeature
    {
        public Guid FeatureId { get; set; }
        public Guid UserId { get; set; }
        public decimal? FeatureValue { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual User User { get; set; }
    }
}