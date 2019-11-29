using Microsoft.Extensions.Options;
using NBB.MultiTenant.Abstractions.Services;
using System;

namespace NBB.MultiTenant.Options
{
    /// <summary>
    /// Tenant aware options cache
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TTenant"></typeparam>
    public class TenantOptionsCache<TOptions, TTenant> : IOptionsMonitorCache<TOptions>
        where TOptions : class
        //where TTenant : Tenant
    {

        private readonly ITenantService _tenantService;
        private readonly TenantOptionsCacheDictionary<TOptions> _tenantSpecificOptionsCache =
            new TenantOptionsCacheDictionary<TOptions>();

        public TenantOptionsCache(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public void Clear()
        {
            var tenant = _tenantService.GetCurrentTenant<TTenant>();
            if (tenant == null)
            {
                return;
            }
            _tenantSpecificOptionsCache.Get(tenant.TenantId.ToString()).Clear();
        }

        public TOptions GetOrAdd(string name, Func<TOptions> createOptions)
        {
            var tenant = _tenantService.GetCurrentTenant<TTenant>();
            
            return _tenantSpecificOptionsCache.Get(tenant.TenantId.ToString())
                .GetOrAdd(name, createOptions);
        }

        public bool TryAdd(string name, TOptions options)
        {
            var tenant = _tenantService.GetCurrentTenant<TTenant>();
            return _tenantSpecificOptionsCache.Get(tenant.TenantId.ToString())
                .TryAdd(name, options);
        }

        public bool TryRemove(string name)
        {
            var tenant = _tenantService.GetCurrentTenant<TTenant>();
            return _tenantSpecificOptionsCache.Get(tenant.TenantId.ToString())
                .TryRemove(name);
        }
    }
}