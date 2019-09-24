using System.Collections.Generic;
using System.Threading.Tasks;
using BasketApi.Models;

namespace BasketApi.Infrastructure.Repositories
{
    public class RedisBasketRepository : IBasketRepository
    {
        public Task<CustomerBasket> GetBasketAsync(string customerId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> GetUsers()
        {
            throw new System.NotImplementedException();
        }

        public Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteBasketAsync(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}