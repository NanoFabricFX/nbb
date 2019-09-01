using System;

namespace NBB.MultiTenant.EntityFramework
{
    public interface IMustHaveTenant
    {
        Guid TenantId { get; set; }
    }
}