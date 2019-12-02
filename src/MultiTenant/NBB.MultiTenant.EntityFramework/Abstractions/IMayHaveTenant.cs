namespace NBB.MultiTenant.EntityFramework.Abstractions
{
    public interface IMayHaveTenant<T>
    {
        T TenantId { get; set; }
    }
}