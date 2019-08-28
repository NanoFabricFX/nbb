using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NBB.MultiTenant.Abstractions;

namespace NBB.MultiTenant.Repositories
{
    public class DatabaseTenantStore : ITenantStore
    {
        private readonly string _connectionString;

        private const string TenantExactFilteredQueryFormat = "SELECT * FROM Tenants WHERE {0} = @{0}";
        private const string TenantLikeFilteredQueryFormat = "SELECT * FROM Tenants WHERE @{0} LIKE {0}";
        private const string TenantInsertFormat = "Insert into Tenants ( Id, Name, Host, SourceIp, ConnectionString, DatabaseClient) values(@Id, @Name, @Host, @SourceIp, @ConnectionString, @DatabaseClient)";
        private const string TenantUpdateFormat = "Update Tenants set Name=  @Name, Host = @Host, SourceIp = @SourceIp, ConnectionString = @ConnectionString, DatabaseClient = @DatabaseClient where Id = @Id";
        private const string TenantDeleteFormat = "Delete from Tenants where Id = @Id";

        public DatabaseTenantStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<Tenant> Get(Guid id)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = string.Format(TenantExactFilteredQueryFormat, nameof(Tenant.Id));
                var result = await connection.QueryAsync<Tenant>(query, new { Id = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<Tenant> GetByName(string name)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = string.Format(TenantExactFilteredQueryFormat, nameof(Tenant.Name));
                var result = await connection.QueryAsync<Tenant>(query, new { Name = name });
                return result.FirstOrDefault();
            }
        }

        public async Task<Tenant> GetByHost(string host)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = string.Format(TenantExactFilteredQueryFormat, nameof(Tenant.Host));
                var result = await connection.QueryAsync<Tenant>(query, new { Host = host });
                return result.FirstOrDefault();
            }
        }

        public async Task<Tenant> GetBySourceIp(string sourceIp)
        {
            if (string.IsNullOrWhiteSpace(sourceIp))
            {
                throw new ArgumentNullException(nameof(sourceIp));
            }

            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = string.Format(TenantLikeFilteredQueryFormat, nameof(Tenant.SourceIp));
                var result = await connection.QueryAsync<Tenant>(query, new { SourceIp = sourceIp });
                return result.FirstOrDefault();
            }
        }

        public async Task<Tenant> GetByHostPort(string host, string ip, int localPort, int remotePort)
        {
            var sourceIp = string.IsNullOrEmpty(ip) ? host : ip;

            if (string.IsNullOrWhiteSpace(sourceIp))
            {
                throw new ArgumentNullException(nameof(sourceIp));
            }

            var ipAndPort = $"{sourceIp}:{localPort}";

            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = string.Format(TenantLikeFilteredQueryFormat, nameof(Tenant.SourceIp));
                var result = await connection.QueryAsync<Tenant>(query, new { SourceIp = ipAndPort });
                return result.FirstOrDefault();
            }
        }

        public async Task<bool> Add(Tenant tenant)
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

        public async Task<bool> Edit(Tenant tenant)
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

        public async Task<bool> Delete(Tenant tenant)
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