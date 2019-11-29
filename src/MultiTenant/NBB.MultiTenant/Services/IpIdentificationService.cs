using Microsoft.AspNetCore.Http;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Services
{
    public class IpIdentificationService : ITenantIdentificationService
    {
        private readonly ITenantStore _store;
        private readonly IHttpContextAccessor _accessor;

        public TenantIdentificationType TenantIdentificationType => TenantIdentificationType.Ip;

        public IpIdentificationService(ITenantStore store, IHttpContextAccessor accessor)
        {
            _store = store;
            _accessor = accessor;
        }

        public async Task<Tenant<T>> GetCurrentTenantAsync<T>()
        {
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            return await _store.GetByHost<T>(ip);
        }

        public Tenant<T> GetCurrentTenant<T>()
        {
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            return _store.GetByHost<T>(ip).GetAwaiter().GetResult();
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