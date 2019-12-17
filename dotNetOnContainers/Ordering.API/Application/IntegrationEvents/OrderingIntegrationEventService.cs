using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLog.Services;
using Ordering.Infrastructure;

namespace Ordering.API.Application.IntegrationEvents
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        //private readonly IIntegrationEventLogService _eventLogService;
        private readonly OrderingContext _orderingContext;

        public OrderingIntegrationEventService(
            IEventBus eventBus, 
            //IIntegrationEventLogService eventLogService, 
            OrderingContext orderingContext)
        {
            _orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            //_eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
        }
        
        public Task PublishEventsThroughEventBusAsync(IntegrationEvent @event)
        {
            try
            {
                _eventBus.Publish(@event);
            }
            catch (Exception ex)
            {
                // ignored
            }

            return null;
        }
        
        /*public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                    
                    _eventBus.Publish(logEvt.IntegrationEvent);
                    
                    await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            await _eventLogService.SaveEventAsync(evt, _orderingContext.GetCurrentTransaction());
        }*/
    }
}