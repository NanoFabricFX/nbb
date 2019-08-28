using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class User
    {
        public User()
        {
            TenantUsers = new HashSet<TenantUser>();
            Tenants = new HashSet<Tenant>();
            UserRoles = new HashSet<UserRole>();
        }

        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public virtual ICollection<TenantUser> TenantUsers { get; set; }
        public virtual ICollection<Tenant> Tenants { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}