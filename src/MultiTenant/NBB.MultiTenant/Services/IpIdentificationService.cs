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

        public async Task<Tenant> GetCurrentTenantAsync()
        {
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            return await _store.GetByHost(ip);
        }

        public Tenant GetCurrentTenant()
        {
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            return _store.GetByHost(ip).GetAwaiter().GetResult();
        }
    }
}