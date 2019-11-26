using Microsoft.AspNetCore.Http;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Pipelines
{
    public class TenantIdentificationMiddleware<T>
    {
        private readonly RequestDelegate _next;

        public TenantIdentificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// This will automatically register the TenantSession.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, ITenantService tenantService, ITenantSession<T> tenantSession)
        {            
            var tenant = await tenantService.GetCurrentTenantAsync<T>();

            if (tenant != null)
            {
                tenantSession.SetTenant(tenant);
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}