namespace NBB.MultiTenant.EntityFramework.Abstractions
{
    public interface IMustHaveTenant<T>
    {
        T TenantId { get; set; }
    }
}