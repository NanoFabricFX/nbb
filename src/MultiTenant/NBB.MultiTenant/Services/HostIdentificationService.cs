using Microsoft.AspNetCore.Http;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Services
{
    public class HostIdentificationService: ITenantIdentificationService
    {
        private readonly ITenantStore _store;
        private readonly HttpContext _context;

        public TenantIdentificationType TenantIdentificationType => TenantIdentificationType.Host;

        public HostIdentificationService(ITenantStore store, IHttpContextAccessor accessor)
        {
            _store = store;
            _context = accessor.HttpContext;
        }       

        public async Task<Tenant> GetCurrentTenant()
        {
            var host = _context.Request.Host.Host;
            return await _store.GetByHost(host);
        }
    }
}