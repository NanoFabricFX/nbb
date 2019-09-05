using Microsoft.AspNetCore.Http;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Services
{
    public class HostIdentificationService : ITenantIdentificationService
    {
        private readonly ITenantStore _store;
        private readonly IHttpContextAccessor _accessor;

        public TenantIdentificationType TenantIdentificationType => TenantIdentificationType.Host;

        public HostIdentificationService(ITenantStore store, IHttpContextAccessor accessor)
        {
            _store = store;
            _accessor = accessor;
        }

        public Tenant GetCurrentTenant()
        {
            var host = _accessor.HttpContext.Request.Host.Host;
            return _store.GetByHost(host).GetAwaiter().GetResult();
        }

        public async Task<Tenant> GetCurrentTenantAsync()
        {
            var host = _accessor.HttpContext.Request.Host.Host;
            return await _store.GetByHost(host);
        }
    }
}