using System;
using System.Threading.Tasks;
using EventBus.Events;

namespace Ordering.API.Application.IntegrationEvents
{
    public interface IOrderingIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}