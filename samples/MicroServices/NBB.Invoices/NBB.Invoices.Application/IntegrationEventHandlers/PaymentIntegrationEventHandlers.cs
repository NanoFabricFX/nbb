﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NBB.Data.Abstractions;
using NBB.Domain.Abstractions;
using NBB.Invoices.Domain.InvoiceAggregate;
using NBB.Payments.PublishedLanguage.IntegrationEvents;

namespace NBB.Invoices.Application.IntegrationEventHandlers
{
    public class PaymentIntegrationEventHandlers : INotificationHandler<PaymentReceived>
    {
        private readonly ICrudRepository<Invoice> _invoiceRepository;

        public PaymentIntegrationEventHandlers(ICrudRepository<Invoice> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }
        public async Task Handle(PaymentReceived e, CancellationToken cancellationToken)
        {
            if (e.InvoiceId == null)
                return;

            var invoice = await _invoiceRepository.GetByIdAsync(e.InvoiceId.Value);
            if (invoice != null)
            {
                invoice.MarkAsPayed(e.PaymentId);

                await _invoiceRepository.SaveChangesAsync();
            }
        }
    }
}