using System;

namespace NBB.MultiTenant.Abstractions
{
    public class Tenant
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; }
    }
}