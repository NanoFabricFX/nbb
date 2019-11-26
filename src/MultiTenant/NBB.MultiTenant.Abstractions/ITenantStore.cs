using System;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Abstractions
{
    public interface ITenantStore
    {
        Task<Tenant<T>> Get<T>(T id);
        Task<Tenant<T>> GetByName<T>(string name);
        Task<Tenant<T>> GetByHost<T>(string host);
        Task<Tenant<T>> GetBySourceIp<T>(string sourceIp);
        Task<Tenant<T>> GetByHostPort<T>(string host, string ip, int localPort, int remotePort);
        Task<bool> Add<T>(Tenant<T> tenant);
        Task<bool> Edit<T>(Tenant<T> tenant);
        Task<bool> Delete<T>(Tenant<T> tenant);
    }
}