using Microsoft.AspNetCore.Http;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Services
{
    public class HostPortIdentificationService : ITenantIdentificationService
    {
        private readonly ITenantStore _store;
        private readonly IHttpContextAccessor _accessor;

        public TenantIdentificationType TenantIdentificationType => TenantIdentificationType.HostPort;

        public HostPortIdentificationService(ITenantStore store, IHttpContextAccessor accessor)
        {
            _store = store;
            _accessor = accessor;
        }

        public Tenant GetCurrentTenant()
        {
            var host = _accessor.HttpContext.Request.Host.Host;
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var localPort = _accessor.HttpContext.Connection.LocalPort;
            var remotePort = _accessor.HttpContext.Connection.RemotePort;
            return _store.GetByHost(host).GetAwaiter().GetResult();
        }

        public async Task<Tenant> GetCurrentTenantAsync()
        {
            var host = _accessor.HttpContext.Request.Host.Host;
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var localPort = _accessor.HttpContext.Connection.LocalPort;
            var remotePort = _accessor.HttpContext.Connection.RemotePort;
            return await _store.GetByHost(host);
        }
    }
}