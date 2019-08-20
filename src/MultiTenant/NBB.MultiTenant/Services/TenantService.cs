using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Services
{
    public class TenantService : ITenantService
    {
        private readonly List<ITenantIdentificationService> _identificationServices;
        private readonly ITenantStore _tenantStore;
        private readonly ITenantSession _session;

        public TenantService(IEnumerable<ITenantIdentificationService> identificationServices, ITenantStore tenantStore, ITenantSession session)
        {
            _identificationServices = identificationServices.ToList();
            _tenantStore = tenantStore;
            _session = session;
        }

        public async Task<bool> Add(Tenant tenant)
        {
            return await _tenantStore.Add(tenant);
        }

        public async Task<bool> Delete(Tenant tenant)
        {
            return await _tenantStore.Delete(tenant);
        }

        public async Task<bool> Edit(Tenant tenant)
        {
            return await _tenantStore.Edit(tenant);
        }

        public async Task<Tenant> GetCurrentTenant()
        {
            if (!_identificationServices.Any())
            {
                throw new Exception("No identification service is configured");
            }

            Tenant tenant = null;
            foreach (var service in _identificationServices)
            {
                tenant = await service.GetCurrentTenant();
                if (tenant != null)
                {
                    _session.SetTenantId(tenant.Id);
                    return tenant;
                }
            }

            return null;
        }
    }
}