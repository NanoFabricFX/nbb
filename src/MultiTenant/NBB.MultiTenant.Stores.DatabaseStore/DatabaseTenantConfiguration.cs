using NBB.MultiTenant.Abstractions;

namespace NBB.MultiTenant.Stores.DatabaseStore
{
    public class DatabaseTenantConfiguration: TenantConfiguration
    {
        public string ConnectionString { get; set; }
    }
}