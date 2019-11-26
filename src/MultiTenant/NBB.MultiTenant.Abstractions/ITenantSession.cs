using System;

namespace NBB.MultiTenant.Abstractions
{
    public interface ITenantSession<T>: IDisposable
    {
        string UserId { get; set; }
        bool IsHostUser { get; }
        bool IsTenantUser { get; }
        bool IsLoggedIn { get; }        
        string ConnectionString { get; }
        void SetTenant(Tenant<T> tenant);
        Tenant<T> GetTenant();

        void SetImpersonatedTenant(Tenant<T> tenant);
        Tenant<T> GetImpersonatedTenant();
    }
}