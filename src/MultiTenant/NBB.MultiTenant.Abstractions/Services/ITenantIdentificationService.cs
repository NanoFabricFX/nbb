using System;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Abstractions.Services
{
    public interface ITenantIdentificationService
    {
        Task<Guid> GetCurrentTenantIdentificationAsync();
    }    
}