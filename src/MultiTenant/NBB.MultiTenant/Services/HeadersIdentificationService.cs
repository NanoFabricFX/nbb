﻿using Microsoft.AspNetCore.Http;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Services
{
    public class HeadersIdentificationService : ITenantIdentificationService
    {
        private readonly string TenantId = "tenantId";
        private readonly ITenantIdentificationService _identificationService;
        private readonly ITenantStore _store;
        private readonly TenantOptions _tenantOptions;
        private readonly HttpContext _context;

        public HeadersIdentificationService(ITenantIdentificationService identificationService, ITenantStore store, TenantOptions tenantOptions, IHttpContextAccessor accessor)
        {
            _identificationService = identificationService;
            _store = store;
            _tenantOptions = tenantOptions;
            _context=accessor.HttpContext;

            if (!string.IsNullOrEmpty(tenantOptions.IdentificationOptions.TenantHeadersKey))
            {
                TenantId = tenantOptions.IdentificationOptions.TenantHeadersKey;
            }
        }

        public TenantIdentificationType TenantIdentificationType => TenantIdentificationType.Headers;

        public async Task<Tenant> GetCurrentTenant()
        {
            var tenantId = _context.Request.Headers[TenantId];

            if (Guid.TryParse(tenantId, out var guid))
            {
                var tenant = await _store.Get(guid);
                return tenant;
            }
            return null;
        }
    }
}