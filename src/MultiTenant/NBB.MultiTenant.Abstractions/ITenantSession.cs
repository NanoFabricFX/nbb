namespace NBB.MultiTenant.Abstractions
{
    public interface ITenantSession
    {
        void SetTenant(Tenant tenant);
        Tenant GetTenant();
        void SetImpersonatedTenant(Tenant tenant);
        Tenant GetImpersonatedTenant();
    }
}