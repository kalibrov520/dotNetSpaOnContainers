using System;
using Autofac;
using EventBus;
using EventBus.Abstractions;
using RabbitMQ.Client;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMq : IEventBus
    {
        private const string BROKER_NAME = "event_bus";

        private readonly IRabbitMqPersistentConnection _connection;
        private readonly ILifetimeScope _autofac;
        private readonly IEventBusSubscriptionManager _manager;
        private readonly string _autofac_scope_name = "event_bus";
        private readonly int _retryCount;

        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMq(IRabbitMqPersistentConnection connection, ILifetimeScope autofac, IEventBusSubscriptionManager manager, string queueName = null, int retryCount = 5)
        {
            _connection = connection;
            _autofac = autofac;
            _manager = manager;
            
        }

        public void Publish(IntegrationEvent @event)
        {
            throw new System.NotImplementedException();
        }

        public void Subscribe<T, TK>() where T : IntegrationEvent where TK : IIntegrationEventHandler<T>
        {
            throw new System.NotImplementedException();
        }

        public void Unsubscribe<T, TK>() where T : IntegrationEvent where TK : IIntegrationEventHandler<T>
        {
            throw new System.NotImplementedException();
        }
    }
}