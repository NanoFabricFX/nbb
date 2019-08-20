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

        public TenantService(IEnumerable<ITenantIdentificationService> identificationServices, ITenantStore tenantStore)
        {
            _identificationServices = identificationServices.ToList();
            _tenantStore = tenantStore;
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
                    return tenant;
                }
            }

            if (tenant == null)
            {
                throw new UnauthorizedAccessException("Could not identify you as a tenant.");
            }

            return null;
        }
    }
}