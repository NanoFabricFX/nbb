using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;

namespace NBB.MultiTenant.Stores.DatabaseStore
{
    public class DatabaseTenantStore : ITenantStore
    {
        private readonly ICryptoService _cryptoService;
        private readonly string _connectionString;

        private const string TenantExactFilteredQueryFormat = "SELECT * FROM Tenants WHERE {0} = @{0}";
        private const string TenantInsertFormat = "Insert into Tenants (TenantId, Name, Host, ConnectionString, DatabaseClient) values(@TenantIdId, @Name, @Host, @ConnectionString, @DatabaseClient)";
        private const string TenantUpdateFormat = "Update Tenants set Name=  @Name, Host = @Host, ConnectionString = @ConnectionString, DatabaseClient = @DatabaseClient where TenantIdId = @TenantIdId";
        private const string TenantDeleteFormat = "Delete from Tenants where Id = @Id";

        public DatabaseTenantStore(DatabaseTenantConfiguration tenantOptions, ICryptoService cryptoService)
        {
            _connectionString = tenantOptions.ConnectionString;
            _cryptoService = cryptoService;            
        }

        private IDbConnection Connection => new SqlConnection(_cryptoService.Decrypt(_connectionString));

        public async Task<Tenant<T>> Get<T>(T id)
        {
            using (var connection = Connection)
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = string.Format(TenantExactFilteredQueryFormat, nameof(Tenant<T>.TenantId));
                var result = await connection.QueryAsync<DatabaseTenant<T>>(query, new { TenantId = id });
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

                var query = string.Format(TenantExactFilteredQueryFormat, nameof(DatabaseTenant<T>.Name));
                var result = await connection.QueryAsync<DatabaseTenant<T>>(query, new { Name = name });
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

                var query = string.Format(TenantExactFilteredQueryFormat, nameof(DatabaseTenant<T>.Host));
                var result = await connection.QueryAsync<DatabaseTenant<T>>(query, new { Host = host });
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