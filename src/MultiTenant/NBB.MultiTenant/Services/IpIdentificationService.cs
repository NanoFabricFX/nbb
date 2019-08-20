﻿using Microsoft.AspNetCore.Http;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Services
{
    public class IpIdentificationService : ITenantIdentificationService
    {
        private readonly ITenantStore _store;
        private readonly HttpContext _context;

        public IpIdentificationService(ITenantStore store, IHttpContextAccessor accessor)
        {
            _store = store;
            _context = accessor.HttpContext;
        }

        public TenantIdentificationType TenantIdentificationType => TenantIdentificationType.Ip;

        public async Task<Tenant> GetCurrentTenant()
        {
            var ip = _context.Connection.RemoteIpAddress.ToString();
            return await _store.GetByHost(ip);
        }        
    }
}