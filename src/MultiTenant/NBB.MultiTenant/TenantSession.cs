using NBB.MultiTenant.Abstractions;
using System;

namespace NBB.MultiTenant
{
    public class TenantSession : ITenantSession
    {
        private Tenant _tenant;

        public string UserId { get; set; }

        public bool IsHostUser => !string.IsNullOrEmpty(UserId) && _tenant == null;

        public bool IsTenantUser => !string.IsNullOrEmpty(UserId) && _tenant != null;

        public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);

        public TenantSession()
        {

        }

        public TenantSession(Tenant tenant)
        {
            _tenant = tenant;
        }

        /// <summary>
        /// Used as a scoped method for impersonation
        /// </summary>
        /// <param name="tenantId"></param>
        public void SetTenantId(Guid tenantId)
        {
            _tenant.Id = tenantId;
        }

        public Guid? GetTenantId()
        {
            return _tenant.Id;
        }

        public void Dispose()
        {
            _tenant = null;
            UserId = null;
        }
    }
}