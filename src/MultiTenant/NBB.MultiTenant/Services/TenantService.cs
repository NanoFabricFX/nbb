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

        public TenantService(IEnumerable<ITenantIdentificationService> identificationServices,
            ITenantStore tenantStore)
        {
            _identificationServices = identificationServices.ToList();
            _tenantStore = tenantStore;
        }

        public async Task<Tenant> GetCurrentTenantAsync()
        {
            if (!_identificationServices.Any())
            {
                throw new Exception("No identification services configured");
            }

            foreach (var service in _identificationServices)
            {
                var tenantId = await service.GetCurrentTenantIdentificationAsync();
                if (tenantId != null)
                {
                    var tenant = await _tenantStore.GetAsync(tenantId);
                    return tenant;
                }
            }

            return null;
        }
    }
}