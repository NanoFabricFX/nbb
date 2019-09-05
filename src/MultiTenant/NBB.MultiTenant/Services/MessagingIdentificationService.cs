using NBB.Messaging.Abstractions;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Services
{
    public class MessagingIdentificationService : ITenantIdentificationService
    {
        private readonly string _tenantIdKey = "tenantId";
        private readonly ITenantStore _store;
        private readonly TenantOptions _tenantOptions;
        private readonly MessagingContextAccessor _messagingContextAccessor;

        public TenantIdentificationType TenantIdentificationType => TenantIdentificationType.Messaging;

        public MessagingIdentificationService(ITenantStore store, TenantOptions tenantOptions, MessagingContextAccessor messagingContextAccessor)
        {
            _store = store;
            _tenantOptions = tenantOptions;
            _messagingContextAccessor = messagingContextAccessor;

        }

        private Guid? GetTenantId()
        {
            var tenantKey = _tenantIdKey;
            if (!string.IsNullOrEmpty(_tenantOptions.IdentificationOptions.TenantHeadersKey))
            {
                tenantKey = _tenantOptions.IdentificationOptions.TenantMessagingKey;
            }

            if (!_messagingContextAccessor.MessagingContext.ReceivedMessageEnvelope.Headers.ContainsKey(tenantKey))
            {
                return null;
            }

            var tenantId = _messagingContextAccessor.MessagingContext.ReceivedMessageEnvelope.Headers[tenantKey];
            if (Guid.TryParse(tenantId, out var guid))
            {
                return guid;
            }
            return null;
        }

        public Tenant GetCurrentTenant()
        {
            var tenantId = GetTenantId();

            if (tenantId.HasValue)
            {
                var tenant = _store.Get(tenantId.Value).GetAwaiter().GetResult();
                return tenant;
            }
            return null;
        }

        public async Task<Tenant> GetCurrentTenantAsync()
        {
            var tenantId = GetTenantId();

            if (tenantId.HasValue)
            {
                var tenant = await _store.Get(tenantId.Value);
                return tenant;
            }
            return null;
        }
    }
}