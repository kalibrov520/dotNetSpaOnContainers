using EventBus.Events;

namespace Basket.API.IntegrationEvents.Events
{
     public class OrderStartedIntegrationEvent : IntegrationEvent
        {
            public string UserId { get; set; }
    
            public OrderStartedIntegrationEvent(string userId)
                => UserId = userId;            
        }
}