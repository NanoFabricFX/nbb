using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Entities
{
    public partial class RoleUserRight
    {
        public Guid RoleId { get; set; }
        public Guid UserRightId { get; set; }

        public virtual Role Role { get; set; }
        public virtual UserRight UserRight { get; set; }
    }
}