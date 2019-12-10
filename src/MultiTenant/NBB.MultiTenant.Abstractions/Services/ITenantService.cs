using System.Threading.Tasks;

namespace NBB.MultiTenant.Abstractions.Services
{
    public interface ITenantService
    {
        Task<Tenant> GetCurrentTenantAsync();
    }
}