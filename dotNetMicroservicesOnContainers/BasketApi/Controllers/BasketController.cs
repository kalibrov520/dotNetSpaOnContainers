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
    }
}