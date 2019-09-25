using System;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Idempotency
{
    public class RequestManager : IRequestManager
    {
        private readonly OrderingContext _context;

        public RequestManager(OrderingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.FindAsync<Request>(id);

            return request != null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {
            var exists = await ExistAsync(id);

            var request = exists
                ? throw new Exception($"Request with {id} exists.")
                : new Request()
                {
                    Id = id,
                    Name = typeof(T).Name,
                    Time = DateTime.Now
                };

            _context.Add(request);

            await _context.SaveChangesAsync();
        }
    }
}