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
            new Booking { Id = 2, EventId = 2, CustomerId = 3, NumberOfTickets = 10},
            new Booking { Id = 3, EventId = 3, CustomerId = 2, NumberOfTickets = 1},
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

        public Booking Add(Booking newBooking)
        {
            // Get Event
            var eventToBook = _eventService.GetById(newBooking.EventId); 
            if (eventToBook == null)
                return FormatBookingError(newBooking, "Event Not Found.", "", -1);

            // Get Number Of Remaining Tickets
            int numberOfTicketsForEvent = _bookings
                .Where(b => b.EventId == newBooking.EventId)
                .Sum(b => b.NumberOfTickets);

            int numberOfTicketsAvailable = eventToBook.Capacity - numberOfTicketsForEvent;

            // Prevent Booking After Date
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            if (eventToBook.Date < today)
                return FormatBookingError(newBooking, "Booking Past Event", "The event you are trying to book has already happened.", numberOfTicketsAvailable);

            // Prevent Overbooking
            if (numberOfTicketsAvailable < 1)
                return FormatBookingError(newBooking, "Event Overbooked", "The event has reached maximum capacity or cannot accommodate this many tickets.", numberOfTicketsAvailable);

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
        Booking FormatBookingError(Booking newBooking, string title, string detail, int numberOfTicketsAvailable)
        {
            newBooking.Error = new BookingError {
                    Title = title,
                    Detail = detail,
                    NumberOfTicketsAvailable = numberOfTicketsAvailable
                };  
            return newBooking;
        }
#endregion
    }
}