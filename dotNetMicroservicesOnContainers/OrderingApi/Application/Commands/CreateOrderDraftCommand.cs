using System.Collections.Generic;
using MediatR;
using OrderingApi.Application.Models;

namespace OrderingApi.Application.Commands
{
    public class CreateOrderDraftCommand : IRequest<OrderDraftDto>
    {
        public string BuyerId { get; private set; }

        public IEnumerable<BasketItem> Items { get; private set; }

        public CreateOrderDraftCommand(string buyerId, IEnumerable<BasketItem> items)
        {
            BuyerId = buyerId;
            Items = items;
        }
    }
}