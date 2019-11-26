using NBB.MultiTenant.Abstractions;
using System;

namespace NBB.MultiTenant
{
    public class TenantSession<T> : ITenantSession<T>
    {
        private bool _isUserImpersonated;
        private bool _isTenantImpersonated;
        private string _userId;

        private string _impersonatorUserId;
        private Tenant<T> _tenant;

        public string UserId
        {
            get
            {
                if (_isUserImpersonated)
                {
                    return _impersonatorUserId;
                }
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }
        

        //public bool IsHostUser => !string.IsNullOrEmpty(UserId) && GetTenant<T> == null;

        //public bool IsTenantUser => !string.IsNullOrEmpty(UserId) && Tenant<T> != null;

        public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);

        //public string ConnectionString => Tenant?.ConnectionString;

        public string ImpersonatorUserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string ConnectionString => throw new NotImplementedException();

        public bool IsHostUser => throw new NotImplementedException();

        public bool IsTenantUser => throw new NotImplementedException();

        public void Dispose()
        {
            UserId = null;
        }

        public Tenant<T> GetImpersonatedTenant()
        {
            throw new NotImplementedException();
        }

        public Tenant<T> GetTenant()
        {
            throw new NotImplementedException();
        }

        public void SetImpersonatedTenant(Tenant<T> tenant)
        {
            throw new NotImplementedException();
        }

        public void SetTenant(Tenant<T> tenant)
        {
            throw new NotImplementedException();
        }
    }
}