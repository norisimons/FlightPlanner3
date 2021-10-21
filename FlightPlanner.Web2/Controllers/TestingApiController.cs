using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using Microsoft.AspNetCore.Mvc;
namespace FlightPlanner.Web2.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class TestingApiController : ControllerBase
    {
        private static readonly object _locker = new();
        private readonly IDbServiceExtended _service;
        public TestingApiController(IDbServiceExtended service)
        {
            _service = service;
        }
        [Route("clear")]
        [HttpPost]
        public IActionResult Clear()
        {
            lock (_locker)
            {
                _service.DeleteAll<Flight>();
                _service.DeleteAll<Airport>();
            }
            return Ok();
        }
    }
}
