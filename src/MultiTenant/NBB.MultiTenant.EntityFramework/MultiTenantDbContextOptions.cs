using Microsoft.EntityFrameworkCore;

namespace NBB.MultiTenant.EntityFramework
{
    public class MultiTenantDbContextOptions<T> : DbContextOptions<T> where T : DbContext
    {
        public bool RestrictCrossTenantAccess { get; set; }
        public bool IdentityTenantByInheritance { get; set; }
        public bool IdentityTenantByAdnotations { get; set; }
    }
}
