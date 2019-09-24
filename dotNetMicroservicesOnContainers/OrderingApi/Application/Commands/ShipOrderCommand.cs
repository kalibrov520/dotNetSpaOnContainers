using System.Runtime.Serialization;
using MediatR;
using OrderingApi.Application.Queries;

namespace OrderingApi.Application.Commands
{
    public class ShipOrderCommand : IRequest<bool>
    {
        [DataMember]
        public int OrderNumber { get; set; }

        public ShipOrderCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}