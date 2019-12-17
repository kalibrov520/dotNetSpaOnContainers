using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.Domain.AggregateModel.BuyerAggregate;
using Ordering.Domain.AggregateModel.OrderAggregate;
using Ordering.Domain.Events;

namespace Ordering.API.Application.DomainEventHandlers.OrderStockConfirmed
{
    public class OrderStatusChangedToStockConfirmedDomainEventHandler
                   : INotificationHandler<OrderStatusChangedToStockConfirmedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public OrderStatusChangedToStockConfirmedDomainEventHandler(
            IOrderRepository orderRepository, 
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _orderingIntegrationEventService = orderingIntegrationEventService;
        }

        public async Task Handle(OrderStatusChangedToStockConfirmedDomainEvent orderStatusChangedToStockConfirmedDomainEvent, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(orderStatusChangedToStockConfirmedDomainEvent.OrderId);
            var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

            var orderStatusChangedToStockConfirmedIntegrationEvent = new OrderStatusChangedToStockConfirmedIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name);
            
            await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(orderStatusChangedToStockConfirmedIntegrationEvent);         
            //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToStockConfirmedIntegrationEvent);            
        }
    }  
}