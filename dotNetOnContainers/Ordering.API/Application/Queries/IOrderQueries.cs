using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.API.Application.Queries
{
    public interface IOrderQueries
    {
        Task<Order> GetOrderAsync(int id);

        Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(Guid userId);

        Task<IEnumerable<CardType>> GetCardTypesAsync();
    }
}