using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ordering.Domain.AggregateModel.OrderAggregate;

namespace Ordering.API.Application.Commands
{
    public class SetStockRejectedOrderStatusCommandHandler : IRequestHandler<SetStockRejectedOrderStatusCommand, bool>
    {        
        private readonly IOrderRepository _orderRepository;

        public SetStockRejectedOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Handler which processes the command when
        /// Stock service rejects the request
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(SetStockRejectedOrderStatusCommand command, CancellationToken cancellationToken)
        {
            // Simulate a work time for rejecting the stock
            await Task.Delay(10000, cancellationToken);

            var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);
            if(orderToUpdate == null)
            {
                return false;
            }            

            orderToUpdate.SetCancelledStatusWhenStockIsRejected(command.OrderStockItems);

            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}