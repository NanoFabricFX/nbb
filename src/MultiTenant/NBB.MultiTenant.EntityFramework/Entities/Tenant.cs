using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class Tenant
    {
        public Tenant()
        {
            TenantUsers = new HashSet<TenantUser>();
            UserRoles = new HashSet<UserRole>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public string SourceIp { get; set; }
        public string ConnectionString { get; set; }
        public int DatabaseClient { get; set; }

        public virtual ICollection<TenantUser> TenantUsers { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}