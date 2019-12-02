namespace NBB.MultiTenant.EntityFramework
{
    public interface IMustHaveTenant<T>
    {
        T TenantId { get; set; }
    }
}