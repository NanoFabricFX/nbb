using System;

namespace NBB.MultiTenant.EntityFramework
{
    public abstract class BaseMultiTenantEntity
    {
        public Guid TenantId { get; set; }
    }
}