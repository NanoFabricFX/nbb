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

        public Tenant<T> GetCurrentTenant<T>()
        {
            var host = _accessor.HttpContext.Request.Host.Host;
            return _store.GetByHost<T>(host).GetAwaiter().GetResult();
        }

        public async Task<Tenant<T>> GetCurrentTenantAsync<T>()
        {
            var host = _accessor.HttpContext.Request.Host.Host;
            return await _store.GetByHost<T>(host);
        }

        public Tenant GetCurrentTenant()
        {
            var tenant = GetCurrentTenant<object>();
            return tenant;
        }

        public async Task<Tenant> GetCurrentTenantAsync()
        {
            var tenant = await GetCurrentTenantAsync<object>();
            return tenant;
        }
    }
}