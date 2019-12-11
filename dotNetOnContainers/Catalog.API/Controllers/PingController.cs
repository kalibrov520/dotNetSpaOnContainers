using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    public class PingController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public string Ping()
        {
            return "[Catalog] Pong...";
        }

    }
}