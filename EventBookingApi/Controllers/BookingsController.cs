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
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public BookingsController(
            IBookingService bookingService,
            IMapper mapper)
        {
            _bookingService = bookingService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all bookings.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingReadDto>))]
        public ActionResult<IEnumerable<BookingReadDto>> GetAll() =>
            Ok(_mapper.Map<IEnumerable<BookingReadDto>>(_bookingService.GetAll()));

        /// <summary>
        /// Returns a booking by its ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingReadDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BookingReadDto> GetById(int id)
        {
            var booking = _bookingService.GetById(id);
            return booking == null ? NotFound(new { message = "Booking not found" }) : _mapper.Map<BookingReadDto>(booking);
        }

        /// <summary>
        /// Adds a new booking.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BookingReadDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<BookingReadDto> Add([FromBody] BookingCreateDto newBookingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Booking booking = _mapper.Map<Booking>(newBookingDto);
            var created = _bookingService.Add(booking);

            if (created.Error != null)
            {
                if (created.Error.NumberOfTicketsAvailable < 0)
                    return NotFound();
                else
                    return Problem(
                    detail: "The event has reached maximum capacity or cannot accommodate this many tickets.",
                            statusCode: StatusCodes.Status409Conflict,
                        title: "Event Overbooked"
                    );
            }

            BookingReadDto readDto = _mapper.Map<BookingReadDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates an existing booking.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Update(int id, [FromBody] BookingCreateDto updatedBookingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_bookingService.Update(id, _mapper.Map<Booking>(updatedBookingDto)))
                return NoContent();

            return BadRequest();
        }
    }
}