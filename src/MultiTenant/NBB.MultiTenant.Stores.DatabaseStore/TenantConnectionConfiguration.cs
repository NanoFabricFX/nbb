using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Data.Abstractions;

namespace NBB.MultiTenant.Stores.DatabaseStore
{   
    public class TenantConnectionConfiguration: TenantConfiguration
    {
        public string ConnectionString { get; set; }
    }
}