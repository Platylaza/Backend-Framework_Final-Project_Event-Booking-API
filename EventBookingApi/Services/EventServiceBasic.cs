using EventBookingApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EventBookingApi.Services
{
    public class EventServiceBasic: IEventService
    {
        private readonly IMemoryCache _cache;
        private readonly List<Event> _events = new List<Event>
        {
            new Event { Id = 1, Name = "Past Event", Date = new DateOnly(2025, 4, 10), Location = "Unknown", Capacity = 100},
            new Event { Id = 2, Name = "Current Event", Date = new DateOnly(2026, 5, 12), Location = "Unknown", Capacity = 100},
            new Event { Id = 3, Name = "Future Event", Date = new DateOnly(2027, 6, 15), Location = "Unknown", Capacity = 100},
        };

        private const string EventsCacheKey = "events_list";

        public EventServiceBasic(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Event> GetAll()
        {
            return null;
        }

        public Event? GetById(int id)
        {
            return null;
        }

        public Event Add(Event newEvent)
        {
            return null;
        }

        public bool Update(int id, Event newEvent)
        {
            return false;
        }

        public bool Delete(int id)
        {
            return false;
        }
    }
}