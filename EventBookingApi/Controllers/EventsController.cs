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

        /// <summary>
        /// Returns a event by its ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventReadDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EventReadDto> GetById(int id)
        {
            var eventObj = _eventService.GetById(id);
            return eventObj == null ? NotFound(new { message = "Event not found" }) : _mapper.Map<EventReadDto>(eventObj);
        }

        /// <summary>
        /// Adds a new event.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EventReadDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<EventReadDto> Add([FromBody] EventCreateDto newEventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Event eventObj = _mapper.Map<Event>(newEventDto);
            var created = _eventService.Add(eventObj);

            EventReadDto readDto = _mapper.Map<EventReadDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates an existing event.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Update(int id, [FromBody] EventCreateDto updatedEventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_eventService.Update(id, _mapper.Map<Event>(updatedEventDto)))
                return NoContent();

            return BadRequest();
        }
    }
}