using System;
using System.Data;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Abstractions
{
    public interface ITenantConnectionFactory
    {
        Task<Func<IDbConnection>> CreateDbConnection();
    }
}