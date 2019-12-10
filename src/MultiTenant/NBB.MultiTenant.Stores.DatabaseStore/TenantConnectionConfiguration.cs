using NBB.MultiTenant.Data.Abstractions;

namespace NBB.MultiTenant.Stores.DatabaseStore
{   
    public class TenantConnectionConfiguration: IConnectionStringConfiguration
    {
        public string ConnectionString { get; set; }

        public string GetConnectionString()
        {
            return ConnectionString;
        }

        public void SetConnectionString(string s)
        {
            ConnectionString = s;
        }
    }
}