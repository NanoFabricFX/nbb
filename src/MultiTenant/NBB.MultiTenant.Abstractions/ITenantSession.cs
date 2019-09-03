using System;

namespace NBB.MultiTenant.Abstractions
{
    public interface ITenantSession : IDisposable
    {
        string UserId { get; set; }
        Guid? TenantId { get; }
        bool IsHostUser { get; }
        bool IsTenantUser { get; }
        bool IsLoggedIn { get; }
        string ImpersonatorUserId { get; set; }
        Guid? ImpersonatedTenantId { get; }
        string ConnectionString { get; }

        Tenant Tenant { get; set; }
        Tenant ImpersonatedTenant { get; set; }
    }
}