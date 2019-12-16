using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents.Events;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    public class OrderPaymentSucceededIntegrationEventHandler : 
        IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
    {
        private readonly IMediator _mediator;

        public OrderPaymentSucceededIntegrationEventHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Handle(OrderPaymentSucceededIntegrationEvent @event)
        {
            var command = new SetPaidOrderStatusCommand(@event.OrderId);

            await _mediator.Send(command);
        }
    }
}