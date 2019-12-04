using NBB.MultiTenant.Abstractions;

namespace NBB.MultiTenant.Stores.DatabaseStore
{
    public class DatabaseTenant<T>: Tenant<T>
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public string ConnectionString { get; set; }
        public DatabaseClient DatabaseClient { get; set; }
    }
}
