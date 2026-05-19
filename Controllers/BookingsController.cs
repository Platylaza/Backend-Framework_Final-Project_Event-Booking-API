using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using AutoMapper;
using EventBookingApi.Models;
using EventBookingApi.DTOs;
using EventBookingApi.Services;
using EventBookingApi.Auth;

namespace EventBookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    //[Authorize(AuthenticationSchemes = ApiKeyAuthenticationHandler.SchemeName)]
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
        /// Returns all bookings by an event's ID.
        /// </summary>
        [HttpGet("event/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingReadDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<BookingReadDto>> GetByEventId(int id)
        {
            var bookings = _bookingService.GetByEventId(id);
            return bookings == null ? NotFound(new { message = "No Bookings found" }) : Ok(_mapper.Map<IEnumerable<BookingReadDto>>(bookings));
        }

        /// <summary>
        /// Returns all bookings by a customer's ID.
        /// </summary>
        [HttpGet("customer/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingReadDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<BookingReadDto>> GetByCustomerId(int id)
        {
            var bookings = _bookingService.GetByCustomerId(id);
            return bookings == null ? NotFound(new { message = "No Bookings found" }) : Ok(_mapper.Map<IEnumerable<BookingReadDto>>(bookings));
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
                if (created.Error.StatusCode == 404)
                    return NotFound();
                else
                    return Problem(
                        detail: created.Error.Detail,
                        statusCode: StatusCodes.Status409Conflict,
                        title: created.Error.Title
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