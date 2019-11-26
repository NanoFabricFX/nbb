using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using NBB.MultiTenant.Abstractions;

namespace NBB.MultiTenant.Repositories
{
    public class DatabaseTenantStore : ITenantStore
    {
        private readonly string _connectionString;

        private const string TenantExactFilteredQueryFormat = "SELECT * FROM Tenants WHERE {0} = @{0}";
        private const string TenantLikeFilteredQueryFormat = "SELECT * FROM Tenants WHERE @{0} LIKE {0}";
        private const string TenantInsertFormat = "Insert into Tenants ( TenantId, Name, Host, SourceIp, ConnectionString, DatabaseClient) values(@TenantIdId, @Name, @Host, @SourceIp, @ConnectionString, @DatabaseClient)";
        private const string TenantUpdateFormat = "Update Tenants set Name=  @Name, Host = @Host, SourceIp = @SourceIp, ConnectionString = @ConnectionString, DatabaseClient = @DatabaseClient where TenantIdId = @TenantIdId";
        private const string TenantDeleteFormat = "Delete from Tenants where Id = @Id";

        public DatabaseTenantStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<Tenant<T>> Get<T>(T id)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = string.Format(TenantExactFilteredQueryFormat, nameof(Tenant<T>.TenantId));
                var result = await connection.QueryAsync<Tenant<T>>(query, new { TenantId = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<Tenant<T>> GetByName<T>(string name)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = string.Format(TenantExactFilteredQueryFormat, nameof(Tenant<T>.Name));
                var result = await connection.QueryAsync<Tenant<T>>(query, new { Name = name });
                return result.FirstOrDefault();
            }
        }

        public async Task<Tenant<T>> GetByHost<T>(string host)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = string.Format(TenantExactFilteredQueryFormat, nameof(Tenant<T>.Host));
                var result = await connection.QueryAsync<Tenant<T>>(query, new { Host = host });
                return result.FirstOrDefault();
            }
        }

        public async Task<Tenant<T>> GetBySourceIp<T>(string sourceIp)
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

                var query = string.Format(TenantLikeFilteredQueryFormat, nameof(Tenant<T>.SourceIp));
                var result = await connection.QueryAsync<Tenant<T>>(query, new { SourceIp = sourceIp });
                return result.FirstOrDefault();
            }
        }

        public async Task<Tenant<T>> GetByHostPort<T>(string host, string ip, int localPort, int remotePort)
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

                var query = string.Format(TenantLikeFilteredQueryFormat, nameof(Tenant<T>.SourceIp));
                var result = await connection.QueryAsync<Tenant<T>>(query, new { SourceIp = ipAndPort });
                return result.FirstOrDefault();
            }
        }

        public async Task<bool> Add<T>(Tenant<T> tenant)
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

        public async Task<bool> Edit<T>(Tenant<T> tenant)
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

        public async Task<bool> Delete<T>(Tenant<T> tenant)
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