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

namespace Ordering.API.Application.DomainEventHandlers.OrderGracePeriodConfirmed
{
    public class OrderStatusChangedToAwaitingValidationDomainEventHandler
                   : INotificationHandler<OrderStatusChangedToAwaitingValidationDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public OrderStatusChangedToAwaitingValidationDomainEventHandler(
            IOrderRepository orderRepository,
            IBuyerRepository buyerRepository,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _buyerRepository = buyerRepository;
            _orderingIntegrationEventService = orderingIntegrationEventService;           
        }

        public async Task Handle(OrderStatusChangedToAwaitingValidationDomainEvent orderStatusChangedToAwaitingValidationDomainEvent, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(orderStatusChangedToAwaitingValidationDomainEvent.OrderId);

            var buyer = await _buyerRepository.FindByIdAsync(order.GetBuyerId.Value.ToString());

            var orderStockList = orderStatusChangedToAwaitingValidationDomainEvent.OrderItems
                .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.GetUnits()));

            var orderStatusChangedToAwaitingValidationIntegrationEvent = new OrderStatusChangedToAwaitingValidationIntegrationEvent(
                order.Id, order.OrderStatus.Name, buyer.Name, orderStockList);
            
            await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(orderStatusChangedToAwaitingValidationIntegrationEvent);

            //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToAwaitingValidationIntegrationEvent);
        }
    }  
}