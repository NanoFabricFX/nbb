using NBB.MultiTenant.Abstractions;

namespace NBB.MultiTenant
{
    public class TenantSession : ITenantSession
    {
        private Tenant _tenant;
        private Tenant _impersonatedTenant;        
        public Tenant GetImpersonatedTenant()
        {
            return _impersonatedTenant;
        }

        public Tenant GetTenant()
        {
            return _impersonatedTenant ?? _tenant;
        }

        public void SetImpersonatedTenant(Tenant tenant)
        {
            _impersonatedTenant = tenant;
        }

        public void SetTenant(Tenant tenant)
        {
            _tenant = tenant;
        }
    }
}