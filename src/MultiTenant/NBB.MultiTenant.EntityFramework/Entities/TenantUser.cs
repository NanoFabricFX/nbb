using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class TenantUser<T>
    {
        public T TenantId { get; set; }
        public T UserId { get; set; }

        public virtual Tenant<T> Tenant { get; set; }
        public virtual User<T> User { get; set; }
    }
}