using Microsoft.Extensions.DependencyInjection;
using NBB.MultiTenant.Abstractions.Services;

namespace NBB.MultiTenant.Cryptography
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddMultiTenantConnectionEncryption<TCryptoServiceType>(this IServiceCollection services, Data.Abstractions.IConnectionStringConfiguration databaseTenantConfiguration)
            where TCryptoServiceType : class, ICryptoService

        {
            services.AddSingleton<ICryptoService, TCryptoServiceType>();
            services.AddSingleton(typeof(Data.Abstractions.IConnectionStringConfiguration), databaseTenantConfiguration);
            services.Decorate<Data.Abstractions.IConnectionStringConfiguration, DatabaseTenantConfigurationDecorator>();
            return services;
        }

        public static IServiceCollection AddMultiTenantAesConnectionEncryption(this IServiceCollection services, Data.Abstractions.IConnectionStringConfiguration databaseTenantConfiguration)
        {
            services.AddSingleton(typeof(Data.Abstractions.IConnectionStringConfiguration), databaseTenantConfiguration);
            services.AddSingleton<ICryptoService, AesCryptoService>();
            services.Decorate<Data.Abstractions.IConnectionStringConfiguration, DatabaseTenantConfigurationDecorator>();
            return services;
        }

        public static IServiceCollection AddMultiTenantNoopConnectionEncryption(this IServiceCollection services, Data.Abstractions.IConnectionStringConfiguration databaseTenantConfiguration)
        {
            services.AddSingleton<ICryptoService, NoopCryptoService>();
            services.AddSingleton(typeof(Data.Abstractions.IConnectionStringConfiguration), databaseTenantConfiguration);
            services.Decorate<Data.Abstractions.IConnectionStringConfiguration, DatabaseTenantConfigurationDecorator>();
            return services;
        }
    }
}