using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class UserRight
    {
        public UserRight()
        {
            RoleUserRights = new HashSet<RoleUserRight>();
        }

        public Guid UserRightId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<RoleUserRight> RoleUserRights { get; set; }
    }
}