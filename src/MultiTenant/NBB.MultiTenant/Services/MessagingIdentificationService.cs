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

            if (!string.IsNullOrEmpty(tenantOptions.IdentificationOptions.TenantHeadersKey))
            {
                _tenantIdKey = tenantOptions.IdentificationOptions.TenantMessagingKey;
            }
        }

        public async Task<Tenant> GetCurrentTenant()
        {
            if (!_messagingContextAccessor.MessagingContext.ReceivedMessageEnvelope.Headers.ContainsKey(_tenantIdKey))
            {
                return null;
            }

            var tenantId = _messagingContextAccessor.MessagingContext.ReceivedMessageEnvelope.Headers[_tenantIdKey];

            if (Guid.TryParse(tenantId, out var guid))
            {
                var tenant = await _store.Get(guid);
                return tenant;
            }

            return null;
        }
    }
}