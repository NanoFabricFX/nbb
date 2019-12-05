using NBB.MultiTenant.Abstractions;
using System;

namespace NBB.MultiTenant.Data.Abstractions
{
    public class TenantDatabaseConfiguration
    {
        public bool UseDefaultValueOnSave { get; set; } = true;
        public bool RestrictCrossTenantAccess { get; set; } = true;

        public bool IsReadOnly { get; set; }        
        public Action<Tenant> ConfigureConnection { get; set; }
    }
}