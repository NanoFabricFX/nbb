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

        public async Task<bool> AddAsync<T>(Tenant<T> tenant)
        {
            return await _tenantStore.Add(tenant);
        }

        public void ImpersonateUser<T>(T userId, Action a)
        {
            //using (var scope = ss.CreateScope())
            //{
            //    var session = scope.ServiceProvider.GetRequiredService<ITenantSession>();
            //    session.ImpersonatedTenant = new Tenant { TenantId = Guid.NewGuid() };
            //    a();
            //}
        }

        public async Task<bool> DeleteAsync<T>(Tenant<T> tenant)
        {
            return await _tenantStore.Delete(tenant);
        }

        public async Task<bool> EditAsync<T>(Tenant<T> tenant)
        {
            return await _tenantStore.Edit(tenant);
        }

        public Tenant<T> GetCurrentTenant<T>()
        {
            if (!_identificationServices.Any())
            {
                throw new Exception("No identification service is configured");
            }

            Tenant<T> tenant = null;
            foreach (var service in _identificationServices)
            {
                tenant = service.GetCurrentTenant<T>();
                if (tenant != null)
                {
                    return tenant;
                }
            }

            return null;
        }

        public async Task<Tenant<T>> GetCurrentTenantAsync<T>()
        {
            if (!_identificationServices.Any())
            {
                throw new Exception("No identification service is configured");
            }

            Tenant<T> tenant = null;
            foreach (var service in _identificationServices)
            {
                tenant = await service.GetCurrentTenantAsync<T>();
                if (tenant != null)
                {
                    return tenant;
                }
            }

            return null;
        }
    }
}