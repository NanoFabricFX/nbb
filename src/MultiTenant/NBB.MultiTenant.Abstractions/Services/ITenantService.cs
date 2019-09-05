using System.Threading.Tasks;

namespace NBB.MultiTenant.Abstractions.Services
{
    public interface ITenantService
    {
        Tenant GetCurrentTenant();
        Task<Tenant> GetCurrentTenantAsync();
        Task<bool> AddAsync(Tenant tenant);
        Task<bool> EditAsync(Tenant tenant);
        Task<bool> DeleteAsync(Tenant tenant);
    }
}