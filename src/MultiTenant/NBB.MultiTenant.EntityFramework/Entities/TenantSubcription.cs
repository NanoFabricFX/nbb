using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class TenantSubcription
    {
        public Guid TenantId { get; set; }
        public Guid SubcriptionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Subscription Subcription { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}