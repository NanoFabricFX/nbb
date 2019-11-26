using System.Threading.Tasks;

namespace NBB.MultiTenant.Abstractions.Services
{
    public interface ITenantIdentificationService
    {
        Tenant<T> GetCurrentTenant<T>();
        Task<Tenant<T>> GetCurrentTenantAsync<T>();
        TenantIdentificationType TenantIdentificationType { get; }
    }

    public enum TenantIdentificationType
    {
        Host = 0,
        Messaging = 1,
        Headers = 2,
        Ip = 3,
        HostPort = 4
    }
}