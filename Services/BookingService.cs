using EventBookingApi.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace EventBookingApi.Services
{
    public class BookingService: IBookingService
    {
        private readonly IEventService _eventService;
        private readonly IMemoryCache _cache;
        private readonly List<Booking> _bookings = new List<Booking>
        {
            new Booking { Id = 1, EventId = 1, CustomerId = 1, NumberOfTickets = 1},
            new Booking { Id = 2, EventId = 1, CustomerId = 3, NumberOfTickets = 2},
            new Booking { Id = 3, EventId = 2, CustomerId = 3, NumberOfTickets = 10},
            new Booking { Id = 4, EventId = 2, CustomerId = 2, NumberOfTickets = 3},
            new Booking { Id = 5, EventId = 3, CustomerId = 2, NumberOfTickets = 1},
        };

        private const string BookingsCacheKey = "bookings_list";

        public BookingService(IMemoryCache cache, IEventService eventService)
        {
            _cache = cache;
            _eventService = eventService;
        }

        public IEnumerable<Booking> GetAll()
        {
            if (!_cache.TryGetValue(BookingsCacheKey, out IEnumerable<Booking> cachedBookings))
            {
                cachedBookings = _bookings.ToList();

                var cachedOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30));

                _cache.Set(BookingsCacheKey, cachedBookings, cachedOptions);
            }

            return cachedBookings;
        }

        public Booking? GetById(int id) => _bookings.FirstOrDefault(e => e.Id == id);
        public IEnumerable<Booking>? GetByEventId(int id) => _bookings.Where(e => e.EventId == id);
        public IEnumerable<Booking>? GetByCustomerId(int id) => _bookings.Where(e => e.CustomerId == id);

        public Booking Add(Booking newBooking)
        {
            throw new Exception("Test Error");

            // Get Event
            var eventToBook = _eventService.GetById(newBooking.EventId); 
            if (eventToBook == null)
                return FormatBookingError(newBooking, new BookingError {
                    Title = "Event Not Found.",
                    Detail = "",
                    NumberOfTicketsAvailable = -1,
                    StatusCode = 404,
                });

            // Get Number Of Remaining Tickets
            int numberOfTicketsForEvent = _bookings
                .Where(b => b.EventId == newBooking.EventId)
                .Sum(b => b.NumberOfTickets);

            int numberOfTicketsAvailable = eventToBook.Capacity - numberOfTicketsForEvent;

            // Prevent Booking After Date
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            if (eventToBook.Date < today)
                return FormatBookingError(newBooking, new BookingError {
                    Title = "Booking Past Event",
                    Detail = "The event you are trying to book has already happened.",
                    NumberOfTicketsAvailable = numberOfTicketsAvailable,
                    StatusCode = 409,
                });

            // Prevent Overbooking
            if (numberOfTicketsAvailable < 1)
                return FormatBookingError(newBooking, new BookingError {
                    Title = "Event Overbooked",
                    Detail = "The event has reached maximum capacity or cannot accommodate this many tickets.",
                    NumberOfTicketsAvailable = numberOfTicketsAvailable,
                    StatusCode = 409,
                });

            // Add Booking
            newBooking.Id = _bookings.Max(e => e.Id) + 1;
            _bookings.Add(newBooking);

            _cache.Remove(BookingsCacheKey);
            return newBooking;
        }

        public bool Update(int id, Booking newBooking)
        {
            var booking = _bookings.FirstOrDefault(e => e.Id == id);
            if (booking == null) return false;

            booking.EventId = newBooking.EventId;
            booking.CustomerId = newBooking.CustomerId;
            booking.NumberOfTickets = newBooking.NumberOfTickets;
            return true;
        }

        public bool Delete(int id)
        {
            var booking = GetById(id);
            if (booking == null) return false;

            _cache.Remove(BookingsCacheKey);
            return _bookings.Remove(booking);
        }

#region Error Handling
        static Booking FormatBookingError(Booking newBooking, BookingError bookingError)
        {
            newBooking.Error = bookingError;  
            return newBooking;
        }
#endregion
    }
}