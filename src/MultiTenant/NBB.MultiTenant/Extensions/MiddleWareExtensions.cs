using Microsoft.AspNetCore.Builder;
using NBB.MultiTenant.Pipelines;

namespace NBB.MultiTenant.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Add the <see cref="TenantIdentificationMiddleware"/> to the request pipeline
        /// This will populate the <see cref="TenantSession"/> object with the value of the current tenant
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTenantIdentificationMiddleware<T>(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantIdentificationMiddleware<T>>();
        }
    }
}