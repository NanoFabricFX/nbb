using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class FeatureUserRight
    {
        public Guid FeatureId { get; set; }
        public Guid UserRightId { get; set; }

        public virtual Feature Feature { get; set; }
        public virtual UserRight UserRight { get; set; }
    }
}