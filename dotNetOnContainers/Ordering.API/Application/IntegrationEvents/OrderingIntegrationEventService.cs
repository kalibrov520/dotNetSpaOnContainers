using System;
using System.Data.Common;
using System.Threading.Tasks;
using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure;

namespace Ordering.API.Application.IntegrationEvents
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        public Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            throw new NotImplementedException();
        }

        public Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            throw new NotImplementedException();
        }
    }
}