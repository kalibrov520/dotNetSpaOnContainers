﻿using System;
using System.Threading.Tasks;
using Basket.API.IntegrationEvents.Events;
using Basket.API.Model;
using EventBus.Abstractions;
using Microsoft.Extensions.Logging;

namespace Basket.API.IntegrationEvents.EventHandling
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly IBasketRepository _repository;

        public OrderStartedIntegrationEventHandler(IBasketRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Handle(OrderStartedIntegrationEvent @event)
        { 
            await _repository.DeleteBasketAsync(@event.UserId.ToString());
        }
    }
}