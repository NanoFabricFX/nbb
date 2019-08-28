using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class TenantUser
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual User User { get; set; }
    }
}