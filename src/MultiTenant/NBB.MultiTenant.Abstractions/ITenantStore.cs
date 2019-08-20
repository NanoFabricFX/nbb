using System;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Abstractions
{
    public interface ITenantStore
    {
        Task<Tenant> Get(Guid id);
        Task<Tenant> GetByName(string name);
        Task<Tenant> GetByHost(string host);
        Task<Tenant> GetBySourceIp(string sourceIp);
        Task<Tenant> GetByHostPort(string host, string ip, int localPort, int remotePort);
        Task<bool> Add(Tenant tenant);
        Task<bool> Edit(Tenant tenant);
        Task<bool> Delete(Tenant tenant);
    }
}