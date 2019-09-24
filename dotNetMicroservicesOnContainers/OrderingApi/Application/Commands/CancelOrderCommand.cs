using System.Runtime.Serialization;
using MediatR;

namespace OrderingApi.Application.Commands
{
    public class CancelOrderCommand : IRequest<bool>
    {
        [DataMember]
        public int OrderNumber { get; set; }

        public CancelOrderCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }
}