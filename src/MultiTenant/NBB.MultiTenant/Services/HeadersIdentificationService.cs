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

        public Tenant<T> GetCurrentTenant<T>()
        {
            var tenantId = GetTenantId<T>();
            var defaultd = default(T);
            if (tenantId.Equals(defaultd))
            {
                var tenant = _store.Get<T>(tenantId).GetAwaiter().GetResult();
                return tenant;
            }
            return null;
        }

        public async Task<Tenant<T>> GetCurrentTenantAsync<T>()
        {
            var tenantId = GetTenantId<T>();
            var defaultd = default(T);
            if (tenantId.Equals(defaultd))
            {
                var tenant = await _store.Get(tenantId);
                return tenant;
            }
            return null;
        }

        private T GetTenantId<T>()
        {
            var tenantKey = _tenantIdKey;
            var context = _accessor.HttpContext;
            if (!string.IsNullOrEmpty(_tenantOptions.IdentificationOptions.TenantHeadersKey))
            {
                tenantKey = _tenantOptions.IdentificationOptions.TenantHeadersKey;
            }

            if (!context.Request.Headers.ContainsKey(tenantKey))
            {
                return default(T);
            }

            var tenantId = context.Request.Headers[tenantKey];
            var tenant = (T)Convert.ChangeType(tenantId, typeof(T));
            return tenant;
        }
    }
}