using System;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Abstractions
{
    public interface ITenantStore
    {
        Task<Tenant> GetAsync(Guid id);
        Task<Tenant> GetByNameAsync(string name);
        Task<bool> AddAsync(Tenant tenant);
        Task<bool> EditAsync(Tenant tenant);
        Task<bool> DeleteAsync(Tenant tenant);
    }
}