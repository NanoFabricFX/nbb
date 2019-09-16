﻿using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class Tenant
    {
        public Tenant()
        {
            TenantFeatures = new HashSet<TenantFeature>();
            TenantSubcriptions = new HashSet<TenantSubcription>();
            TenantUsers = new HashSet<TenantUser>();
            UserRoles = new HashSet<UserRole>();
        }

        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public string SourceIp { get; set; }
        public string ConnectionString { get; set; }
        public int DatabaseClient { get; set; }
        public Guid OwnerId { get; set; }

        public virtual User Owner { get; set; }
        public virtual ICollection<TenantFeature> TenantFeatures { get; set; }
        public virtual ICollection<TenantSubcription> TenantSubcriptions { get; set; }
        public virtual ICollection<TenantUser> TenantUsers { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}