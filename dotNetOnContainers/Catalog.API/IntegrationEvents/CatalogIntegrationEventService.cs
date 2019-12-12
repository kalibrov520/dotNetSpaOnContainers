using System;
using System.Data.Common;
using System.Threading.Tasks;
using Catalog.API.Infrastructure;
using EventBus.Abstractions;
using EventBus.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.API.IntegrationEvents
{
    public class CatalogIntegrationEventService : ICatalogIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly CatalogContext _catalogContext;

        public CatalogIntegrationEventService(
            IEventBus eventBus,
            CatalogContext catalogContext)
        {
            _catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
            {
                _eventBus.Publish(evt);
            }
            catch (Exception ex)
            {
                //ignored
            }
        }

        public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt)
        {
            await _catalogContext.SaveChangesAsync();
        }
    }
}