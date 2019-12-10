using Dapper;
using Microsoft.Data.SqlClient;
using NBB.MultiTenant.Abstractions;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Stores.DatabaseStore
{
    public class DatabaseTenantStore : ITenantStore
    {
        private readonly string _connectionString;

        private const string TenantExactFilteredQueryFormat = "SELECT TenantIdId, Name, Host, ConnectionString, DatabaseClient FROM Tenants WHERE {0} = @{0}";
        private const string TenantInsertFormat = "Insert into Tenants (TenantId, Name, Host, ConnectionString, DatabaseClient) values(@TenantIdId, @Name, @Host, @ConnectionString, @DatabaseClient)";
        private const string TenantUpdateFormat = "Update Tenants set Name=  @Name, Host = @Host, ConnectionString = @ConnectionString, DatabaseClient = @DatabaseClient where TenantIdId = @TenantIdId";
        private const string TenantDeleteFormat = "Delete from Tenants where Id = @Id";

        public DatabaseTenantStore(TenantConnectionConfiguration tenantConnectionConfiguration)
        {
            _connectionString = tenantConnectionConfiguration.ConnectionString;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<Tenant> GetAsync(Guid id)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = string.Format(TenantExactFilteredQueryFormat, nameof(Tenant.TenantId));
                var result = await connection.QueryAsync<DatabaseTenant>(query, new { TenantId = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<Tenant> GetByNameAsync(string name)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = string.Format(TenantExactFilteredQueryFormat, nameof(DatabaseTenant.Name));
                var result = await connection.QueryAsync<DatabaseTenant>(query, new { Name = name });
                return result.FirstOrDefault();
            }
        }

        public async Task<bool> AddAsync(Tenant tenant)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                var count = await Connection.ExecuteAsync(TenantInsertFormat, tenant);
                return count == 1;
            }
        }

        public async Task<bool> EditAsync(Tenant tenant)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                var count = await Connection.ExecuteAsync(TenantUpdateFormat, tenant);
                return count == 1;
            }
        }

        public async Task<bool> DeleteAsync(Tenant tenant)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                var count = await Connection.ExecuteAsync(TenantDeleteFormat, tenant);
                return count == 1;
            }
        }
    }
}