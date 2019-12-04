using NBB.MultiTenant.Abstractions;

namespace NBB.MultiTenant
{
    public class TenantSession<T> : ITenantSession<T>
    {
        private Tenant<T> _tenant;
        private Tenant<T> _impersonatedTenant;        
        public Tenant<T> GetImpersonatedTenant()
        {
            return _impersonatedTenant;
        }

        public Tenant<T> GetTenant()
        {
            return _impersonatedTenant ?? _tenant;
        }

        Tenant ITenantSession.GetTenant()
        {
            return _impersonatedTenant?? _tenant;
        }

        public void SetImpersonatedTenant(Tenant<T> tenant)
        {
            _impersonatedTenant = tenant;
        }

        public void SetImpersonatedTenant(Tenant tenant)
        {
            _impersonatedTenant = tenant as Tenant<T>;
        }

        public void SetTenant(Tenant<T> tenant)
        {
            _tenant = tenant;
        }

        public void SetTenant(Tenant tenant)
        {
            _tenant = tenant as Tenant<T>;
        }

        Tenant ITenantSession.GetImpersonatedTenant()
        {
            return _impersonatedTenant;
        }
    }
}