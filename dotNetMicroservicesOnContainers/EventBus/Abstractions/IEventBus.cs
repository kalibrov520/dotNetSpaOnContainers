namespace EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void Subscribe<T, TK>() where T : IntegrationEvent where TK : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TK>() where T : IntegrationEvent where TK : IIntegrationEventHandler<T>;
    }
}