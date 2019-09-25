using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Ordering.Domain.Common;

namespace Ordering.Infrastructure
{
    static class MediatorExtension
    {
        public static async Task DispatchDomainEventAsync(this IMediator mediator, OrderingContext context)
        {
            var domainEntities = context.ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();

            domainEntities.ToList().ForEach(x => x.Entity.ClearDomainEvents());

            var tasks = domainEvents.Select(async (domainEvent) => { await mediator.Publish(domainEvent); });

            await Task.WhenAll(tasks);
        }
    }
}