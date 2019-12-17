using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure.Idempotency;

namespace Ordering.API.Application.Commands
{
    public class IdentifiedCommandHandler<T, R> : IRequestHandler<IdentifiedCommand<T, R>, R>
        where T : IRequest<R>
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;

        public IdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager)
        {
            _mediator = mediator;
            _requestManager = requestManager;
        }

        /// <summary>
        /// Creates the result value to return if a previous request was found
        /// </summary>
        /// <returns></returns>
        protected virtual R CreateResultForDuplicateRequest()
        {
            return default(R);
        }

        /// <summary>
        /// This method handles the command. It just ensures that no other request exists with the same ID, and if this is the case
        /// just enqueues the original inner command.
        /// </summary>
        /// <param name="message">IdentifiedCommand which contains both original command & request ID</param>
        /// <returns>Return value of inner command or default value if request same ID was found</returns>
        public async Task<R> Handle(IdentifiedCommand<T, R> message, CancellationToken cancellationToken)
        {
            var alreadyExists = await _requestManager.ExistAsync(message.Id);
            if (alreadyExists)
            {
                return CreateResultForDuplicateRequest();
            }
            else
            {
                await _requestManager.CreateRequestForCommandAsync<T>(message.Id);
                try
                {
                    var command = message.Command;
                    var idProperty = string.Empty;
                    var commandId = string.Empty;

                    switch (command)
                    {
                        case CreateOrderCommand createOrderCommand:
                            idProperty = nameof(createOrderCommand.UserId);
                            commandId = createOrderCommand.UserId;
                            break;

                        case CancelOrderCommand cancelOrderCommand:
                            idProperty = nameof(cancelOrderCommand.OrderNumber);
                            commandId = $"{cancelOrderCommand.OrderNumber}";
                            break;

                        case ShipOrderCommand shipOrderCommand:
                            idProperty = nameof(shipOrderCommand.OrderNumber);
                            commandId = $"{shipOrderCommand.OrderNumber}";
                            break;

                        default:
                            idProperty = "Id?";
                            commandId = "n/a";
                            break;
                    }

                    // Send the embeded business command to mediator so it runs its related CommandHandler 
                    var result = await _mediator.Send(command, cancellationToken);

                    return result;
                }
                catch
                {
                    return default(R);
                }
            }
        }
    }
}