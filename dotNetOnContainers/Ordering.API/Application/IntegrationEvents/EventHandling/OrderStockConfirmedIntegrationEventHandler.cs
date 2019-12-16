using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents.Events;

namespace Ordering.API.Application.IntegrationEvents.EventHandling
{
    public class OrderStockConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>
    {
        private readonly IMediator _mediator;

        public OrderStockConfirmedIntegrationEventHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Handle(OrderStockConfirmedIntegrationEvent @event)
        {
            var command = new SetStockConfirmedOrderStatusCommand(@event.OrderId);
            
            await _mediator.Send(command);
        }
    }
}