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
            int eventId = newBooking.EventId;
            var eventObj = _eventService.GetById(eventId); 
            if (eventObj == null)
            {
                newBooking.Error = new BookingError {
                    Message = "Event Not Found",
                    NumberOfTicketsAvailable = -1
                }; 
                return newBooking;
            }

            // Prevent Overbooking
            int numberOfTicketsForEvent = _bookings
                .Where(b => b.EventId == newBooking.EventId)
                .Sum(b => b.NumberOfTickets);

            if (numberOfTicketsForEvent + newBooking.NumberOfTickets > eventObj.Capacity) 
            {
                newBooking.Error = new BookingError {
                    Message = "There is no room for those tickets.",
                    NumberOfTicketsAvailable = eventObj.Capacity - numberOfTicketsForEvent
                };  
                return newBooking;
            }

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
    }
}