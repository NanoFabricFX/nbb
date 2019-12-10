using NBB.Messaging.Abstractions;
using NBB.Messaging.DataContracts;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Messaging.Decorators
{
    public class MultiTenantPublisherDecorator<TKey> : IMessageBusPublisher
    {
        private readonly IMessageBusPublisher _inner;
        private readonly ITenantService _tenantService;
        private readonly TenantMessagingConfiguration _tenantMessagingConfiguration;

        public MultiTenantPublisherDecorator(IMessageBusPublisher inner, ITenantService tenantService, TenantMessagingConfiguration tenantMessagingConfiguration)
        {
            _inner = inner;
            _tenantService = tenantService;
            _tenantMessagingConfiguration = tenantMessagingConfiguration;
        }

        public Task PublishAsync<T>(T message, CancellationToken cancellationToken, Action<MessagingEnvelope> customizer = null, string topicName = null)
        {
            void NewCustomizer(MessagingEnvelope outgoingEnvelope)
            {
                var tenant = _tenantService.GetCurrentTenantAsync().GetAwaiter().GetResult();
                if (tenant != null)
                {
                    outgoingEnvelope.SetHeader(_tenantMessagingConfiguration.TenantMessagingKey, tenant.TenantId.ToString());
                }

                customizer?.Invoke(outgoingEnvelope);
            }

            return _inner.PublishAsync(message, cancellationToken, NewCustomizer);
        }
    }
}