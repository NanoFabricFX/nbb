namespace NBB.MultiTenant.Abstractions
{
    public class Tenant<T>: Tenant, ITenant<T>
    {
        public new T TenantId { get; set; }
    }

    public class Tenant
    {
        public object TenantId { get; set; }
    }
}