using NBB.MultiTenant.Abstractions;

namespace NBB.MultiTenant.EntityFramework.Abstractions
{
    public interface IMustHaveTenant<T> : ITenant<T>
    {
        new T TenantId { get; set; }
    }
}