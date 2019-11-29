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
        Host = 0,
        MessagingHeaders = 1,
        Headers = 2,
        Ip = 3,
        HostPort = 4
    }
}