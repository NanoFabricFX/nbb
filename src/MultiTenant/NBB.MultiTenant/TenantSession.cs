using NBB.MultiTenant.Abstractions;
using System;

namespace NBB.MultiTenant
{
    public class TenantSession : ITenantSession
    {
        private Guid? _tenantId;
        private bool _isUserImpersonated;
        private bool _isTenantImpersonated;
        private string _userId;

        private string _impersonatorUserId;
        private Guid? _impersonatedTenantId;

        public string UserId
        {
            get
            {
                if (_isTenantImpersonated)
                {
                    return ImpersonatorUserId;
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
                if (_isTenantImpersonated)
                {
                    return ImpersonatedTenantId;
                }
                return _tenantId;
            }
            set
            {
                _tenantId = value;
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
                _isUserImpersonated = true;
                UserId = value;
            }
        }

        public Guid? ImpersonatedTenantId
        {
            get
            {
                return _impersonatedTenantId;
            }
            set
            {
                _isTenantImpersonated = true;
                TenantId = value;
            }
        }

        public bool IsHostUser => !string.IsNullOrEmpty(UserId) && !_tenantId.HasValue;

        public bool IsTenantUser => !string.IsNullOrEmpty(UserId) && _tenantId.HasValue;

        public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);

        public void Use(Guid? tenantId, string userId)
        {
            _isTenantImpersonated = true;
            _isUserImpersonated = true;
            TenantId = tenantId;
            UserId = userId;
        }

        public void Dispose()
        {
            UserId = null;
        }
    }
}