using Microsoft.Extensions.DependencyInjection;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using NBB.MultiTenant.Repositories;
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
        public static IServiceCollection AddMultiTenantServices(this IServiceCollection services, TenantOptions tenantOptions)
        {
            if (tenantOptions.TenantStoreType == TenantStoreType.Sql)
            {
                services.AddScoped<ITenantStore>(provider => new DatabaseTenantStore(tenantOptions.ConnectionString));
            }

            services.AddScoped<ITenantConnectionFactory, TenantConnectionFactory>();
            if (tenantOptions.UseConnectionStringEncryption)
            {
                services.AddScoped<ICryptoService>(provider => new AesCryptoService(tenantOptions.EncryptionKey));
            }
            else
            {
                services.AddScoped<ICryptoService, NoopCryptoService>();
            }
            
            services.AddScoped<ITenantService, TenantService>();

            if (tenantOptions.IdentificationOptions.UseHeaders)
            {
                services.AddScoped<ITenantIdentificationService, HeadersIdentificationService>();
            }

            if (tenantOptions.IdentificationOptions.UseHost)
            {
                services.AddScoped<ITenantIdentificationService, HostIdentificationService>();
            }

            if (tenantOptions.IdentificationOptions.UseMessagingHeaders)
            {
                services.AddScoped<ITenantIdentificationService, MessagingIdentificationService>();
            }

            services.AddScoped(provider =>
            {
                var connectionFactory = provider.GetService<ITenantConnectionFactory>();
                return connectionFactory.CreateDbConnection().GetAwaiter().GetResult();
            });

            services.AddSingleton(s=> tenantOptions);
            services.AddScoped<ITenantSession, TenantSession>();
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