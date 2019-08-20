using System;

namespace NBB.MultiTenant.EntityFramework
{
    public interface IMayHaveTenant
    {
        Guid? TenantId { get; set; }
    }
}