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

        public TenantIdentificationType TenantIdentificationType => TenantIdentificationType.MessagingHeaders;

        public MessagingIdentificationService(ITenantStore store, TenantOptions tenantOptions, MessagingContextAccessor messagingContextAccessor)
        {
            _store = store;
            _tenantOptions = tenantOptions;
            _messagingContextAccessor = messagingContextAccessor;
        }

        private T GetTenantId<T>()
        {
            var tenantKey = _tenantIdKey;
            if (!string.IsNullOrEmpty(_tenantOptions.IdentificationOptions.TenantHeadersKey))
            {
                tenantKey = _tenantOptions.IdentificationOptions.TenantMessagingKey;
            }

            if (!_messagingContextAccessor.MessagingContext.ReceivedMessageEnvelope.Headers.ContainsKey(tenantKey))
            {
                return default;
            }

            var tenantId = _messagingContextAccessor.MessagingContext.ReceivedMessageEnvelope.Headers[tenantKey];
            return (T)Convert.ChangeType(tenantId, typeof(T));
        }

        public Tenant<T> GetCurrentTenant<T>()
        {
            var tenantId = GetTenantId<T>();

            if (tenantId.Equals(default(T)))
            {
                return default;
            }

            var tenant = _store.Get(tenantId).GetAwaiter().GetResult();
            return tenant;

        }

        public async Task<Tenant<T>> GetCurrentTenantAsync<T>()
        {
            var tenantId = GetTenantId<T>();

            if (tenantId != default)
            {
                var tenant = await _store.Get(tenantId);
                return tenant;
            }
            return null;
        }

        public Tenant GetCurrentTenant()
        {
            var tenant = GetCurrentTenant<object>();
            return tenant;
        }

        public async Task<Tenant> GetCurrentTenantAsync()
        {
            var tenant = await GetCurrentTenantAsync<object>();
            return tenant;
        }
    }
}