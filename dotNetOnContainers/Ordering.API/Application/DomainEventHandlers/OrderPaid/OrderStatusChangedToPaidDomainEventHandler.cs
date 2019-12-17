using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.Domain.AggregateModel.BuyerAggregate;
using Ordering.Domain.AggregateModel.OrderAggregate;
using Ordering.Domain.Events;

namespace Ordering.API.Application.DomainEventHandlers.OrderPaid
{
    public class OrderStatusChangedToPaidDomainEventHandler
                   : INotificationHandler<OrderStatusChangedToPaidDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        

        public OrderStatusChangedToPaidDomainEventHandler(
            IOrderRepository orderRepository,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService
            )
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));            
        }

        public async Task Handle(OrderStatusChangedToPaidDomainEvent orderStatusChangedToPaidDomainEvent, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(orderStatusChangedToPaidDomainEvent.OrderId);
            var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

            var orderStockList = orderStatusChangedToPaidDomainEvent.OrderItems
                .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()));

            var orderStatusChangedToPaidIntegrationEvent = new OrderStatusChangedToPaidIntegrationEvent(
                orderStatusChangedToPaidDomainEvent.OrderId,
                order.OrderStatus.Name,
                buyer.Name,
                orderStockList);

            await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(orderStatusChangedToPaidIntegrationEvent);
            //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToPaidIntegrationEvent);         
        }
    }  
}