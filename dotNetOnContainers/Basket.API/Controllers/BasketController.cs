using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Basket.API.IntegrationEvents.Events;
using Basket.API.Model;
using EventBus.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    public class BasketController : Controller
    {
        private readonly IBasketRepository _repository;
        private readonly IEventBus _eventBus;
        private readonly ILogger<BasketController> _logger;

        public BasketController(
            ILogger<BasketController> logger,
            IBasketRepository repository,
            IEventBus eventBus)
        {
            _logger = logger;
            _repository = repository;
            _eventBus = eventBus;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
        {
            var basket = await _repository.GetBasketAsync(id);

            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody]CustomerBasket value)
        {
            return Ok(await _repository.UpdateBasketAsync(value));
        }

        [Route("checkout/:userId")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CheckoutAsync(string userId,[FromBody]BasketCheckout basketCheckout, [FromHeader(Name = "x-requestid")] string requestId)
        {
            /*basketCheckout.RequestId = (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty) ?
                guid : basketCheckout.RequestId;

            var basket = await _repository.GetBasketAsync(userId);

            if (basket == null)
            {
                return BadRequest();
            }*/
            var basket = new CustomerBasket();

            var userName = "test";

            var eventMessage = new UserCheckoutAcceptedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street,
                basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName,
                basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basketCheckout.RequestId, basket);

            _eventBus.Publish(eventMessage);

            return Accepted();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteBasketByIdAsync(string id)
        {
            await _repository.DeleteBasketAsync(id);
        }
    }
}