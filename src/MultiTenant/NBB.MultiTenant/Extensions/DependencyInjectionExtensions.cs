using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using NBB.MultiTenant.Options;
using NBB.MultiTenant.Repositories;
using NBB.MultiTenant.Services;
using System;
using System.Collections.Concurrent;

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
        public static IServiceCollection AddMultiTenantServices<TKey>(this IServiceCollection services, TenantOptions tenantOptions)
        {
            if (tenantOptions.TenantStoreType == TenantStoreType.Sql)
            {
                services.AddSingleton<ITenantStore>(provider => new DatabaseTenantStore(tenantOptions.ConnectionString));
            }

            services.AddScoped<ITenantConnectionFactory<TKey>, TenantConnectionFactory<TKey>>();
            if (tenantOptions.UseConnectionStringEncryption)
            {
                services.AddSingleton<ICryptoService>(provider => new AesCryptoService(tenantOptions.EncryptionKey));
            }
            else
            {
                services.AddSingleton<ICryptoService, NoopCryptoService>();
            }

            services.AddSingleton<ITenantService, TenantService>();

            if (tenantOptions.IdentificationOptions.RegisteredServices?.Count == 0 && tenantOptions.IdentificationOptions.IdentitificationTypes?.Count == 0)
            {
                tenantOptions.IdentificationOptions.WithDefaultOptions();
            }

            if (tenantOptions.IdentificationOptions.ShoudUse(TenantIdentificationType.Headers))
            {
                tenantOptions.IdentificationOptions.AddIdentificationService<HeadersIdentificationService>();
            }

            if (tenantOptions.IdentificationOptions.ShoudUse(TenantIdentificationType.Host))
            {
                tenantOptions.IdentificationOptions.AddIdentificationService<HostIdentificationService>();
            }

            if (tenantOptions.IdentificationOptions.ShoudUse(TenantIdentificationType.MessagingHeaders))
            {
                tenantOptions.IdentificationOptions.AddIdentificationService<MessagingIdentificationService>();
            }

            if (tenantOptions.IdentificationOptions.ShoudUse(TenantIdentificationType.Ip))
            {
                tenantOptions.IdentificationOptions.AddIdentificationService<IpIdentificationService>();
            }

            if (tenantOptions.IdentificationOptions.ShoudUse(TenantIdentificationType.HostPort))
            {
                tenantOptions.IdentificationOptions.AddIdentificationService<HostPortIdentificationService>();
            }


            foreach (var serviceType in tenantOptions.IdentificationOptions.RegisteredServices)
            {
                services.AddSingleton(typeof(ITenantIdentificationService), serviceType);
            }

            services.AddScoped(provider =>
            {
                var connectionFactory = provider.GetService<ITenantConnectionFactory<TKey>>();
                return connectionFactory.CreateDbConnection().GetAwaiter().GetResult();
            });

            services.AddSingleton(s => tenantOptions);
            //services.AddScoped<ITenantSession>(s =>
            //{
            //    using (var scope = s.CreateScope())
            //    {
            //        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
            //        var tenant = tenantService.GetCurrentTenant();
            //        if (tenant == null)
            //        {
            //            return null;
            //        }
            //        var tenantSession = new TenantSession
            //        {
            //            Tenant = tenant
            //        };
            //        return tenantSession;
            //    }

            //});
            services.AddScoped<ITenantSession<TKey>, TenantSession<TKey>>();
            services.AddScoped<ITenantSession, TenantSession<TKey>>();
            return services;
        }

        public static IServiceCollection AddTenantFeatureResolver(this IServiceCollection services, TenantOptions tenantOptions)
        {
            ////var services = new ServiceCollection();
            //services.AddScoped<ExampleService>();
            //var globalProvider = services.BuildServiceProvider();

            //using (var scope = globalProvider.CreateScope())
            //{
            //    var localScoped = scope.ServiceProvider.GetService<ExampleService>();

            //    var globalScoped = globalProvider.GetService<ExampleService>();
            //}
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

        //private static ConcurrentDictionary<TenantSingletonKey<string>, object> _cache = new ConcurrentDictionary<TenantSingletonKey<string>, object>();

        //public static IServiceCollection AddPerTenantSingleton<TInterface, TImplementation, TTenantKey>(this IServiceCollection services, ITenantService tenantService) where TImplementation : class, TInterface where TInterface : class
        //{
        //    services.AddSingleton<TInterface, TImplementation>((s) =>
        //    {

        //        var tenant = tenantService.GetCurrentTenant<TTenantKey>();
        //        var key = new TenantSingletonKey<string>
        //        {
        //            TenantId = tenant.TenantId.ToString(),
        //            Interface = typeof(TInterface)
        //        };
        //        var impl = _cache.GetOrAdd(key, (u) =>
        //        {
        //            var implementation = ActivatorUtilities.CreateInstance<TImplementation>(services);
        //            return implementation;
        //        });
        //        return (TImplementation)impl;
        //    });
        //    return services;
        //}
    }

    class TenantSingletonKey<T>
    {
        public T TenantId { get; set; }
        public Type Interface { get; set; }
    }
    //class ExampleService
    //{

    //}

    //public class TenantServiceProvider : IServiceProvider
    //{
    //    private readonly IServiceCollection _services;
    //    private readonly ITenantService _tenantService;
    //    private readonly IServiceProvider _globalProvider;
    //    private readonly System.Collections.Generic.Dictionary<Guid, IServiceScope> scopes = new System.Collections.Generic.Dictionary<Guid, IServiceScope>();


    //    public TenantServiceProvider(IServiceCollection services, ITenantService tenantService)
    //    {
    //        _services = services;
    //        _tenantService = tenantService;
    //        _globalProvider = services.BuildServiceProvider();
    //    }

    //    public object GetService(Type serviceType)
    //    {
    //        var tenant = _tenantService.GetCurrentTenant().GetAwaiter().GetResult();
    //        if (tenant == null)
    //        {
    //            return _globalProvider.GetRequiredService(serviceType);
    //        }
    //        throw new NotImplementedException();
    //    }
    //}
}