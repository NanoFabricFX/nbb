using System;

namespace NBB.MultiTenant.EntityFramework.Abstractions
{
    public interface IMayHaveTenant
    {
        public Guid TenantId { get; set; }
    }
}