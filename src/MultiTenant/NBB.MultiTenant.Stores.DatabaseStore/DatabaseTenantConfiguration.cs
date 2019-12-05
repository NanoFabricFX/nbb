using NBB.MultiTenant.Abstractions;
using System;

namespace NBB.MultiTenant.Stores.DatabaseStore
{
    public class DatabaseTenantConfiguration: TenantConfiguration
    {
        public string ConnectionString { get; set; }       
       
        
    }
}