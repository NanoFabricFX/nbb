using System.Threading.Tasks;

namespace NBB.MultiTenant.Abstractions.Services
{
    public interface ITenantService
    {
        Tenant<T> GetCurrentTenant<T>();
        Task<Tenant<T>> GetCurrentTenantAsync<T>();
        Task<bool> AddAsync<T>(Tenant<T> tenant);
        Task<bool> EditAsync<T>(Tenant<T> tenant);
        Task<bool> DeleteAsync<T>(Tenant<T> tenant);
    }
}