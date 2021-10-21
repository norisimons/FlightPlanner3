using AutoMapper;
using FlightPlanner.Core.Dto;
using FlightPlanner.Core.Models;
using FlightPlanner.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
namespace FlightPlanner.Web2.Controllers
{
    [Authorize]
    [Route("admin-api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private static readonly object _locker = new();
        private readonly IFlightService _flightService;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IValidator> _validators;

        public AdminApiController(IFlightService flightService, IMapper mapper, IEnumerable<IValidator> validators)
        {
            _flightService = flightService;
            _mapper = mapper;
            _validators = validators;
        }

        [HttpGet]
        [Route("flights/{id}")]
        public IActionResult GetFlight(int id)
        {
            lock (_locker)
            {
                var flight = _flightService.GetFullFlightById(id);
                if (flight == null)
                    return NotFound();
                return Ok(_mapper.Map<FlightResponse>(flight));
            }
        }

        [HttpPut]
        [Route("flights")]
        public IActionResult PutFlight(FlightRequest request)
        {
            lock (_locker)
            {
                if (!_validators.All(s => s.IsValid(request)))
                {
                    return BadRequest();
                }

                var flight = _mapper.Map<Flight>(request);
                if (_flightService.Exists(flight))
                {
                    return Conflict();
                }

                _flightService.Create(flight);
                return Created("", _mapper.Map<FlightResponse>(flight));
            }
        }

        [HttpDelete]
        [Route("flights/{id}")]
        public IActionResult DeleteFlight(int id)
        {
            var flight = _flightService.GetFullFlightById(id);

            if (flight != null)
            {
                _flightService.DeleteFlight(id);
            }
            return Ok();
        }
    }
}

