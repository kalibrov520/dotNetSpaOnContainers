using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderingApi.Application.Queries
{
    public class OrderQueries : IOrderQueries
    {
        public Task<Order> GetOrderAsync(int id)
        {
            throw new NotImplementedException();
        }

        //TODO: int -> Guid
        public Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CardType>> GetCardTypesAsync()
        {
            throw new NotImplementedException();
        }
    }
}