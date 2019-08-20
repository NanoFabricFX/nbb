using NBB.MultiTenant.Abstractions;
using System;

namespace NBB.MultiTenant.Attributes
{
    public class RequireHostUserAttibute: Attribute
    {
        public RequireHostUserAttibute(ITenantSession tenantSession)
        {
            if (!tenantSession.IsHostUser)
            {
                throw new Exception("You must be logged in as a host user");
            }
        }
    }
}