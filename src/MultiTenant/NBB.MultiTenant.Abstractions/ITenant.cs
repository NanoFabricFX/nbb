namespace NBB.MultiTenant.Abstractions
{
    public interface ITenant<T>
    {
        public T TenantId { get; set; }
    }
}