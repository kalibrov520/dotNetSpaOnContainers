using System.Collections.Generic;
using OrderingApi.Application.Queries;

namespace OrderingApi.Application.Commands
{
    public class CreateOrderDraftCommandHandler
    {
        
    }

    public class OrderDraftDto
    {
        public IEnumerable<OrderItemDto> OrderItems { get; set; }
        public decimal Total { get; set; }

        public static OrderDraftDto FromOrder(Order order)
        {
            return new OrderDraftDto()
            {
                //TODO: implement
            };
        }

    }
}