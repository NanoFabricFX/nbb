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

        public HeadersIdentificationService(ITenantStore store, TenantOptions tenantOptions, IHttpContextAccessor accessor)
        {
            _store = store;
            _tenantOptions = tenantOptions;
            _accessor = accessor;
        }

        public TenantIdentificationType TenantIdentificationType => TenantIdentificationType.Headers;

        public async Task<Tenant> GetCurrentTenant()
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
                var tenant = await _store.Get(guid);
                return tenant;
            }
            return null;
        }
    }
}