using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using EventBookingApi.Models;
using EventBookingApi.DTOs;
using EventBookingApi.Services;

namespace EventBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;

        public EventsController(
            IEventService eventService,
            IMapper mapper)
        {
            _eventService = eventService;
            _mapper = mapper;
        }

        // CRUD :     | Create | Read | Update | Delete
        // Http OPs : | Post   | Get  | Put    | Delete

        /// <summary>
        /// Returns all events.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EventReadDto>))]
        public ActionResult<IEnumerable<EventReadDto>> GetAll() =>
            Ok(_mapper.Map<IEnumerable<EventReadDto>>(_eventService.GetAll()));
    }
}