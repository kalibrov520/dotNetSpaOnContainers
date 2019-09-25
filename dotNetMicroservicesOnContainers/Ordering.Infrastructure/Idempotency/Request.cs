using System;

namespace Ordering.Infrastructure.Idempotency
{
    public class Request
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
    }
}