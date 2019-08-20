using System;

namespace NBB.MultiTenant.Abstractions
{
    public interface ITenantSession : IDisposable
    {
        void SetTenantId(Guid tenantId);
        Guid? GetTenantId();
        bool IsHostUser { get; }
        bool IsTenantUser { get; }
        bool IsLoggedIn { get; }
    }
}