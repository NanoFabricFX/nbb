using System.Threading.Tasks;

namespace NBB.MultiTenant.Abstractions.Services
{
    public interface ITenantService
    {
        Task<Tenant> GetCurrentTenant();
        Task<bool> Add(Tenant tenant);
        Task<bool> Edit(Tenant tenant);
        Task<bool> Delete(Tenant tenant);
    }
}