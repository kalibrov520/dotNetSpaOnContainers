using System;
using System.Net;
using System.Threading.Tasks;
using BasketApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BasketApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repo;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IBasketRepository repo, ILogger<BasketController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
        {
            var basket = await _repo.GetBasketAsync(id);

            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody]CustomerBasket value)
        {
            return Ok(await _repo.UpdateBasketAsync(value));
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteBasketByIdAsync(string id)
        {
            await _repo.DeleteBasketAsync(id);
        }

        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.Accepted)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout,
            [FromHeader(Name = "x-requestid")] string requestId)
        {
            // TODO: Implement after EventBus adding.
            throw new NotImplementedException();
        }
    }
}