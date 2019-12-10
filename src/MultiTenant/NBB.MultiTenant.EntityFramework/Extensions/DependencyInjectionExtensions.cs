using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NBB.Core.Abstractions;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;

using System;
using System.Collections.Generic;

namespace NBB.MultiTenant.EntityFramework.Extensions
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds to services collection the required services to make the multitenancy work
        /// </summary>
        /// <returns>Services collection</returns>
        public static IServiceCollection AddEfMultiTenantServices<TKey, TStoreType>(this IServiceCollection services, IEnumerable<ITenantIdentificationService> identificationServices)
            where TStoreType : class, ITenantStore
        {
            services.AddScoped<IUow<>, EfUow<>>();
            services
            .Decorate(typeof(IUow<>), typeof(MultitenantUowDecorator<>));

            
            return services;
        }
    }
}
