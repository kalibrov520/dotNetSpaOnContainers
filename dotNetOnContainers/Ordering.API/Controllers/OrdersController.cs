﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.API.Application.Commands;
using Ordering.API.Application.Queries;

namespace Ordering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IOrderQueries _orderQueries;

        public OrdersController(
            IMediator mediator, 
            IOrderQueries orderQueries)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
        }

        [Route("cancel")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CancelOrderAsync([FromBody]CancelOrderCommand command)
        {
            bool commandResult = false;

            commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Route("ship")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ShipOrderAsync([FromBody]ShipOrderCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            bool commandResult = false;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestShipOrder = new IdentifiedCommand<ShipOrderCommand, bool>(command, guid);

                commandResult = await _mediator.Send(requestShipOrder);
            }

            if (!commandResult)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Route("{orderId:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(Order),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetOrderAsync(int orderId)
        {
            try
            {
                //Todo: It's good idea to take advantage of GetOrderByIdQuery and handle by GetCustomerByIdQueryHandler
                //var order customer = await _mediator.Send(new GetOrderByIdQuery(orderId));
                var order = await _orderQueries.GetOrderAsync(orderId);

                return Ok(order);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderSummary>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrdersAsync(string userId)
        {
            var orders = await _orderQueries.GetOrdersFromUserAsync(Guid.Parse(userId));

            return Ok(orders);
        }

        [Route("cardtypes")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CardType>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<CardType>>> GetCardTypesAsync()
        {
            var cardTypes = await _orderQueries.GetCardTypesAsync();

            return Ok(cardTypes);
        }

        [Route("draft")]
        [HttpPost]
        public async Task<ActionResult<OrderDraftDto>> CreateOrderDraftFromBasketDataAsync([FromBody] CreateOrderDraftCommand createOrderDraftCommand)
        {
            return await _mediator.Send(createOrderDraftCommand);
        }
    }
}