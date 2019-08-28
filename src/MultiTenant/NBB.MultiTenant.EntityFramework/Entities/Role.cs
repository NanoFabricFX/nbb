using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class Role
    {
        public Role()
        {
            RoleUserRights = new HashSet<RoleUserRight>();
            UserRoles = new HashSet<UserRole>();
        }

        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<RoleUserRight> RoleUserRights { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}