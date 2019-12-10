using System;

namespace NBB.MultiTenant.EntityFramework.Abstractions
{
    public interface IMustHaveTenant
    {
        public Guid TenantId { get; set; }
    }
}