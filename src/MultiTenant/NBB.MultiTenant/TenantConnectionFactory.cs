using NBB.MultiTenant.Abstractions;
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NBB.MultiTenant.Abstractions.Services;

namespace NBB.MultiTenant
{
    public sealed class TenantConnectionFactory : ITenantConnectionFactory
    {
        private readonly ITenantService _tenantService;
        private readonly ICryptoService _cryptoService;

        public TenantConnectionFactory(ITenantService tenantService, ICryptoService cryptoService)
        {
            _tenantService = tenantService;
            _cryptoService = cryptoService;
        }

        public async Task<Func<IDbConnection>> CreateDbConnection()
        {
            var tenant = await _tenantService.GetCurrentTenantAsync();
            var tenantConnectionString = _cryptoService.Decrypt(tenant.ConnectionString);

            switch (tenant.DatabaseClient)
            {
                case DatabaseClient.SqlClient:
                    return () => new SqlConnection(tenantConnectionString);
                default:
                    return () => new SqlConnection(tenantConnectionString);
            }
        }
    }
}