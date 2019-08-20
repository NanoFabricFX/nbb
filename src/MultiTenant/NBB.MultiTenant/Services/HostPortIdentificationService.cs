using Microsoft.AspNetCore.Http;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Services
{
    public class HostPortIdentificationService: ITenantIdentificationService
    {
        private readonly ITenantStore _store;
        private readonly HttpContext _context;

        public TenantIdentificationType TenantIdentificationType => TenantIdentificationType.HostPort;

        public HostPortIdentificationService(ITenantStore store, IHttpContextAccessor accessor)
        {
            _store = store;
            _context = accessor.HttpContext;
        }       

        public async Task<Tenant> GetCurrentTenant()
        {
            var host = _context.Request.Host.Host;
            var ip = _context.Connection.RemoteIpAddress.ToString();
            var localPort = _context.Connection.LocalPort;
            var remotePort = _context.Connection.RemotePort;
            return await _store.GetByHost(host);
        }
    }
}