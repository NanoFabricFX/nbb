using NBB.Messaging.Abstractions;
using NBB.Messaging.DataContracts;
using NBB.MultiTenant.Abstractions;
using NBB.MultiTenant.Abstractions.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.MultiTenant.Messaging
{
    public class MultiTenantPublisherDecorator : IMessageBusPublisher
    {
        private readonly IMessageBusPublisher _inner;
        private readonly ITenantService _tenantService;
        private readonly TenantOptions _tenantOptions;

        public MultiTenantPublisherDecorator(IMessageBusPublisher inner, ITenantService tenantService, TenantOptions tenantOptions)
        {
            _inner = inner;
            _tenantService = tenantService;
            _tenantOptions = tenantOptions;
        }

        public Task PublishAsync<T>(T message, CancellationToken cancellationToken, Action<MessagingEnvelope> customizer = null, string topicName = null)
        {
            void NewCustomizer(MessagingEnvelope outgoingEnvelope)
            {
                var tenant = _tenantService.GetCurrentTenant();
                if (tenant != null)
                {
                    outgoingEnvelope.SetHeader(_tenantOptions.IdentificationOptions.TenantMessagingKey, tenant.Id.ToString());
                }

                customizer?.Invoke(outgoingEnvelope);
            }

            return _inner.PublishAsync(message, cancellationToken, NewCustomizer);
        }
    }
}