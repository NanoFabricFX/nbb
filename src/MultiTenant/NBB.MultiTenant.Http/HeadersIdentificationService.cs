using Microsoft.AspNetCore.Http;
using NBB.MultiTenant.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Http
{
    public class HeadersIdentificationService : ITenantIdentificationService
    {
        private readonly string _tenantIdKey = "tenantId";
        private readonly TenantHttpOptions _tenantHttpOptions;
        private readonly IHttpContextAccessor _accessor;

        public HeadersIdentificationService(TenantHttpOptions tenantHttpOptions, IHttpContextAccessor accessor)
        {        
            _tenantHttpOptions = tenantHttpOptions;
            _accessor = accessor;
        }

        public Task<Guid> GetCurrentTenantIdentificationAsync()
        {
            var tenantKey = _tenantIdKey;
            var context = _accessor.HttpContext;
            if (!string.IsNullOrEmpty(_tenantHttpOptions.TenantHeadersKey))
            {
                tenantKey = _tenantHttpOptions.TenantHeadersKey;
            }

            if (!context.Request.Headers.ContainsKey(tenantKey))
            {
                return Task.FromResult(default(Guid));
            }

            var tenantId = context.Request.Headers[tenantKey];
            if (Guid.TryParse(tenantId, out var guid))
            {
                return Task.FromResult(guid);
            }
            return Task.FromResult(default(Guid));
        }
    }
}