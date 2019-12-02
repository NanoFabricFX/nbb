namespace NBB.MultiTenant.EntityFramework
{
    public interface IMayHaveTenant<T>
    {
        T TenantId { get; set; }
    }
}