using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ordering.API.Application.IntegrationEvents;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.Domain.AggregateModel.BuyerAggregate;
using Ordering.Domain.Events;

namespace Ordering.API.Application.DomainEventHandlers.OrderStarted
{
    public class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(IBuyerRepository buyerRepository, IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
        }

        public async Task Handle(OrderStartedDomainEvent orderStartedEvent, CancellationToken cancellationToken)
        {
            var cardTypeId = (orderStartedEvent.CardTypeId != 0) ? orderStartedEvent.CardTypeId : 1;
            var buyer = await _buyerRepository.FindAsync(orderStartedEvent.UserId);
            var buyerOriginallyExisted = (buyer != null);

            if (!buyerOriginallyExisted)
            {
                buyer = new Buyer(orderStartedEvent.UserId, orderStartedEvent.UserName);
            }

            buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                           $"Payment Method on {DateTime.UtcNow}",
                                           orderStartedEvent.CardNumber,
                                           orderStartedEvent.CardSecurityNumber,
                                           orderStartedEvent.CardHolderName,
                                           orderStartedEvent.CardExpiration,
                                           orderStartedEvent.Order.Id);

            await _buyerRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);

            var orderStatusChangedToSubmittedIntegrationEvent = new OrderStatusChangedToSubmittedIntegrationEvent(orderStartedEvent.Order.Id, orderStartedEvent.Order.OrderStatus.Name, buyer.Name);
            await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync(orderStatusChangedToSubmittedIntegrationEvent);
            //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToSubmittedIntegrationEvent);
        }
    }
}