using System;

namespace NBB.MultiTenant.Abstractions
{
    public interface ITenantSession<T> : ITenantSession
    {
        new void SetTenant(Tenant<T> tenant);
        new Tenant<T> GetTenant();
        new void SetImpersonatedTenant(Tenant<T> tenant);
        new Tenant<T> GetImpersonatedTenant();
    }

    public interface ITenantSession
    {
        void SetTenant(Tenant tenant);
        Tenant GetTenant();
        void SetImpersonatedTenant(Tenant tenant);
        Tenant GetImpersonatedTenant();
    }
}