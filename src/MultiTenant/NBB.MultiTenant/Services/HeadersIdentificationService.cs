using Microsoft.AspNetCore.Http;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Services
{
    public class HeadersIdentificationService : ITenantIdentificationService
    {
        private readonly string _tenantIdKey = "tenantId";
        private readonly ITenantStore _store;
        private readonly TenantOptions _tenantOptions;
        private readonly IHttpContextAccessor _accessor;

        public TenantIdentificationType TenantIdentificationType => TenantIdentificationType.Headers;

        public HeadersIdentificationService(ITenantStore store, TenantOptions tenantOptions, IHttpContextAccessor accessor)
        {
            _store = store;
            _tenantOptions = tenantOptions;
            _accessor = accessor;
        }        

        public Tenant GetCurrentTenant()
        {
            var tenantId = GetTenantId();
            if (tenantId.HasValue)
            {
                var tenant = _store.Get(tenantId.Value).GetAwaiter().GetResult();
                return tenant;
            }
            return null;
        }

        public async Task<Tenant> GetCurrentTenantAsync()
        {
            var tenantId = GetTenantId();
            if (tenantId.HasValue)
            {
                var tenant = await _store.Get(tenantId.Value);
                return tenant;
            }
            return null;
        }

        private Guid? GetTenantId()
        {
            var tenantKey = _tenantIdKey;
            var context = _accessor.HttpContext;
            if (!string.IsNullOrEmpty(_tenantOptions.IdentificationOptions.TenantHeadersKey))
            {
                tenantKey = _tenantOptions.IdentificationOptions.TenantHeadersKey;
            }

            if (!context.Request.Headers.ContainsKey(tenantKey))
            {
                return null;
            }

            var tenantId = context.Request.Headers[tenantKey];

            if (Guid.TryParse(tenantId, out var guid))
            {
                return guid;
            }
            return null;
        }
    }
}