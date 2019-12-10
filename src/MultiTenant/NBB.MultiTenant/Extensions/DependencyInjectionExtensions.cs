using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using NBB.MultiTenant.Services;
using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.Extensions
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds to services collection the required services to make the multitenancy work
        /// </summary>
        /// <returns>Services collection</returns>
        public static IServiceCollection AddMultiTenantServices<TKey, TStoreType>(this IServiceCollection services, IEnumerable<ITenantIdentificationService> identificationServices) 
            where TStoreType: class, ITenantStore            
        {            
            services.AddSingleton<ITenantStore, TStoreType>();
            services.AddSingleton<ITenantService, TenantService>();          

            services.AddScoped(provider =>
            {
                var connectionFactory = provider.GetService<ITenantConnectionFactory>();
                return connectionFactory.CreateDbConnection().GetAwaiter().GetResult();
            });

            TenantSession factory(IServiceProvider s)
            {
                using (var scope = s.CreateScope())
                {
                    var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
                    var tenant = tenantService.GetCurrentTenantAsync().GetAwaiter().GetResult();
                    if (tenant == null)
                    {
                        return null;
                    }
                    var tenantSession = new TenantSession();
                    tenantSession.SetTenant(tenant);
                    return tenantSession;
                }
            }

            services.AddScoped<ITenantSession>(factory);            
            return services;
        }
    }    
}