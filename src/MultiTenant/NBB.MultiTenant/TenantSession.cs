using NBB.MultiTenant.Abstractions;
using System;

namespace NBB.MultiTenant
{
    public class TenantSession : ITenantSession
    {
        private bool _isUserImpersonated;
        private bool _isTenantImpersonated;
        private string _userId;

        private string _impersonatorUserId;

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

        public Guid? TenantId
        {
            get
            {
                if (ImpersonatedTenant != null)
                {
                    return ImpersonatedTenant.TenantId;
                }
                return Tenant?.TenantId;
            }
        }

        public string ImpersonatorUserId
        {
            get
            {
                return _impersonatorUserId;
            }
            set
            {
                _isUserImpersonated = string.IsNullOrEmpty(value);
                _impersonatorUserId = value;
            }
        }


        public Guid? ImpersonatedTenantId => ImpersonatedTenant?.TenantId;

        public Tenant ImpersonatedTenant { get; set; }

        public Tenant Tenant { get; set; }

        public bool IsHostUser => !string.IsNullOrEmpty(UserId) && Tenant == null;

        public bool IsTenantUser => !string.IsNullOrEmpty(UserId) && Tenant != null;

        public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);

        public string ConnectionString => Tenant?.ConnectionString;



        public void Dispose()
        {
            UserId = null;
        }
    }
}