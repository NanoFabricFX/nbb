using System;

namespace NBB.MultiTenant.Abstractions
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public string SourceIp { get; set; }
        public string ConnectionString { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public DatabaseClient DatabaseClient { get; set; }
    }

    public enum DatabaseClient
    {
        SqlClient = 0,
        MySql = 1
    }
}