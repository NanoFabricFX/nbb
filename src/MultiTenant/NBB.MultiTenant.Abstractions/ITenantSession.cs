using System;

namespace NBB.MultiTenant.Abstractions
{
    public interface ITenantSession : IDisposable
    {
        string UserId { get; set; }
        Guid? TenantId { get; set; }
        void Use(Guid? tenantId, string userId);
        bool IsHostUser { get; }
        bool IsTenantUser { get; }
        bool IsLoggedIn { get; }
        string ImpersonatorUserId { get; set; }
        Guid? ImpersonatedTenantId { get; set; }
    }
}