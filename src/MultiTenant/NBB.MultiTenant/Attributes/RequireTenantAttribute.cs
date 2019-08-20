using NBB.MultiTenant.Abstractions;
using System;

namespace NBB.MultiTenant.Attributes
{
    public class RequireTenantAttribute : Attribute
    {
        public RequireTenantAttribute(ITenantSession tenantSession)
        {
            if (!tenantSession.IsTenantUser)
            {
                throw new Exception("You must be logged in as a tenant user");
            }
        }
    }
}