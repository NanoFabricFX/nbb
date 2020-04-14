using System;
using System.Threading.Tasks;

namespace NBB.MultiTenancy.Identification.Identifiers
{
    public class IdTenantIdentifier : ITenantIdentifier
    {
        public Task<Guid?> GetTenantIdAsync(string tenantToken)
        {
            return Guid.TryParse(tenantToken, out var tenantId) ? Task.FromResult((Guid?)tenantId) : null;
        }
    }
}
