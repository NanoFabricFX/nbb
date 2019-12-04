using System.Threading.Tasks;

namespace NBB.MultiTenant.Abstractions.Services
{
    public interface ITenantIdentificationService
    {
        Tenant GetCurrentTenant();
        Task<Tenant> GetCurrentTenantAsync();
        Tenant<T> GetCurrentTenant<T>();
        Task<Tenant<T>> GetCurrentTenantAsync<T>();
        TenantIdentificationType TenantIdentificationType { get; }
    }

    public enum TenantIdentificationType
    {
        Custom = 0,
        Host = 1,
        MessagingHeaders = 2,
        Headers = 3,        
        HostPort = 4
    }
}