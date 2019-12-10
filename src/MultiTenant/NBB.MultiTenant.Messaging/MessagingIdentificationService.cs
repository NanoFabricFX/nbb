using NBB.Messaging.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Messaging
{
    public class MessagingIdentificationService : ITenantIdentificationService
    {
        private readonly string _tenantIdKey = "tenantId";
        private readonly TenantMessagingConfiguration _tenantMessagingConfiguration;
        private readonly MessagingContextAccessor _messagingContextAccessor;

        public MessagingIdentificationService(TenantMessagingConfiguration tenantMessagingConfiguration, MessagingContextAccessor messagingContextAccessor)
        {
            _tenantMessagingConfiguration = tenantMessagingConfiguration;
            _messagingContextAccessor = messagingContextAccessor;
        }        

        public Task<Guid> GetCurrentTenantIdentificationAsync()
        {
            var tenantKey = _tenantIdKey;
            if (!string.IsNullOrEmpty(_tenantMessagingConfiguration.TenantMessagingKey))
            {
                tenantKey = _tenantMessagingConfiguration.TenantMessagingKey;
            }

            if (!_messagingContextAccessor.MessagingContext.ReceivedMessageEnvelope.Headers.ContainsKey(tenantKey))
            {
                return default;
            }

            var tenantId = _messagingContextAccessor.MessagingContext.ReceivedMessageEnvelope.Headers[tenantKey];
            if (Guid.TryParse(tenantId, out var guid)){
                return Task.FromResult(guid);
            }
            return Task.FromResult(default(Guid));
        }
    }
}