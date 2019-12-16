﻿using System.Threading.Tasks;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.AggregateModel.OrderAggregate
{
    public interface IOrderRepository : IRepository<Order>
    {
        Order Add(Order order);
        
        void Update(Order order);

        Task<Order> GetAsync(int orderId);
    }
}