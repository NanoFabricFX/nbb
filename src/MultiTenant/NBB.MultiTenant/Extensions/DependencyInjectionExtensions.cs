using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using NBB.MultiTenant.Options;
using NBB.MultiTenant.Services;
using System;

namespace NBB.MultiTenant.Extensions
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds to services collection the required services to make the multitenancy work
        /// </summary>
        /// <param name="services">Services collection</param>
        /// <param name="connectionString">Connection string or blob storage parameters</param>
        /// <param name="encryptionKey">Key to encrypt connection string</param>
        /// <returns>Services collection</returns>
        public static IServiceCollection AddMultiTenantServices<TKey>(this IServiceCollection services, TenantConfiguration tenantConfiguration)
        {            
            services.AddSingleton(typeof(ITenantStore), tenantConfiguration.TenantStoreType);
            services.AddSingleton(typeof(ICryptoService), tenantConfiguration.CryptoServiceType);

            services.AddSingleton<ITenantService, TenantService>();

            if (tenantConfiguration.IdentificationOptions.RegisteredServices?.Count == 0 && tenantConfiguration.IdentificationOptions.IdentitificationTypes?.Count == 0)
            {
                tenantConfiguration.IdentificationOptions.WithDefaultOptions();
            }

            if (tenantConfiguration.IdentificationOptions.ShoudUse(TenantIdentificationType.Headers))
            {
                tenantConfiguration.IdentificationOptions.AddIdentificationService<HeadersIdentificationService>();
            }

            if (tenantConfiguration.IdentificationOptions.ShoudUse(TenantIdentificationType.Host))
            {
                tenantConfiguration.IdentificationOptions.AddIdentificationService<HostIdentificationService>();
            }

            if (tenantConfiguration.IdentificationOptions.ShoudUse(TenantIdentificationType.MessagingHeaders))
            {
                tenantConfiguration.IdentificationOptions.AddIdentificationService<MessagingIdentificationService>();
            }

            if (tenantConfiguration.IdentificationOptions.ShoudUse(TenantIdentificationType.HostPort))
            {
                tenantConfiguration.IdentificationOptions.AddIdentificationService<HostPortIdentificationService>();
            }

            foreach (var serviceType in tenantConfiguration.IdentificationOptions.RegisteredServices)
            {
                services.AddSingleton(typeof(ITenantIdentificationService), serviceType);
            }

            services.AddScoped(provider =>
            {
                var connectionFactory = provider.GetService<ITenantConnectionFactory<TKey>>();
                return connectionFactory.CreateDbConnection().GetAwaiter().GetResult();
            });

            services.AddSingleton(s => tenantConfiguration);
            TenantSession<TKey> factory(IServiceProvider s)
            {
                using (var scope = s.CreateScope())
                {
                    var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
                    var tenant = tenantService.GetCurrentTenant();
                    if (tenant == null)
                    {
                        return null;
                    }
                    var tenantSession = new TenantSession<TKey>();
                    tenantSession.SetTenant(tenant);
                    return tenantSession;
                }
            }

            services.AddScoped<ITenantSession>(factory);
            services.AddScoped<ITenantSession<TKey>, TenantSession<TKey>>(factory);
            return services;
        }



        /// <summary>
        /// Register tenant specific options
        /// </summary>
        /// <typeparam name="TOptions">Type of options we are apply configuration to</typeparam>
        /// <param name="tenantOptionsConfiguration">Action to configure options for a tenant</param>
        /// <returns></returns>
        public static IServiceCollection WithPerTenantOptions<TOptions, T>(this IServiceCollection services, Action<TOptions, T> tenantConfig) where TOptions : class, new() where T : class, new()
        {
            //Register the multi-tenant cache
            services.AddSingleton<IOptionsMonitorCache<TOptions>>(a => ActivatorUtilities.CreateInstance<TenantOptionsCache<TOptions, T>>(a));

            //Register the multi-tenant options factory
            services.AddTransient<IOptionsFactory<TOptions>>(a => ActivatorUtilities.CreateInstance<TenantOptionsFactory<TOptions, T>>(a, tenantConfig));

            //Register IOptionsSnapshot support
            services.AddScoped<IOptionsSnapshot<TOptions>>(a => ActivatorUtilities.CreateInstance<TenantOptions<TOptions>>(a));

            //Register IOptions support
            services.AddSingleton<IOptions<TOptions>>(a => ActivatorUtilities.CreateInstance<TenantOptions<TOptions>>(a));

            return services;
        }

    }    
}