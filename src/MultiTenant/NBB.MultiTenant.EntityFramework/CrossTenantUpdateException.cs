using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework
{
    public class CrossTenantUpdateException<T> : ApplicationException
    {
        public IList<T> TenantIds { get; private set; }

        public CrossTenantUpdateException(IList<T> tenantIds)
        {
            TenantIds = tenantIds;
        }
    }
}