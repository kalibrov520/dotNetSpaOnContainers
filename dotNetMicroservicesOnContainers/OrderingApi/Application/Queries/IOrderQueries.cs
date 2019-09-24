using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderingApi.Application.Queries
{
    public interface IOrderQueries
    {
        Task<Order> GetOrderAsync(int id);

        //TODO: int -> Guid
        Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(int userId);

        Task<IEnumerable<CardType>> GetCardTypesAsync();
    }
}