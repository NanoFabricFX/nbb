using System;
using System.Collections.Generic;
using System.Text;

namespace NBB.MultiTenant.EntityFramework
{
    public interface IMustHaveTenant
    {
        Guid TenantId { get; set; }
    }
}
