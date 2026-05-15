using EventBookingApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EventBookingApi.Services
{
    public class EventService: IEventService
    {
        private readonly IMemoryCache _cache;
        private readonly List<Event> _events = new List<Event>
        {
            new Event { Id = 1, Name = "Past Event", Date = new DateOnly(2025, 4, 10), Location = "Unknown", Capacity = 5},
            new Event { Id = 2, Name = "Current Event", Date = new DateOnly(2026, 5, 15), Location = "Unknown", Capacity = 15},
            new Event { Id = 3, Name = "Future Event", Date = new DateOnly(2027, 6, 14), Location = "Unknown", Capacity = 2},
            new Event { Id = 4, Name = "Future Event 2", Date = new DateOnly(2028, 7, 13), Location = "Unknown", Capacity = 5},
        };

        private const string EventsCacheKey = "events_list";

        public EventService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Event> GetAll()
        {
            if (!_cache.TryGetValue(EventsCacheKey, out IEnumerable<Event> cachedEvents))
            {
                cachedEvents = _events.ToList();

                var cachedOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30));

                _cache.Set(EventsCacheKey, cachedEvents, cachedOptions);
            }

            return cachedEvents;
        }

        public Event? GetById(int id) => _events.FirstOrDefault(e => e.Id == id);

        public Event Add(Event newEvent)
        {
            newEvent.Id = _events.Max(e => e.Id) + 1;
            _events.Add(newEvent);

            _cache.Remove(EventsCacheKey);

            return newEvent;
        }

        public bool Update(int id, Event newEvent)
        {
            var eventObj = _events.FirstOrDefault(e => e.Id == id);
            if (eventObj == null) return false;

            eventObj.Name = newEvent.Name;
            eventObj.Date = newEvent.Date;
            eventObj.Location = newEvent.Location;
            eventObj.Capacity = newEvent.Capacity;
            return true;
        }

        public bool Delete(int id)
        {
            var eventObj = GetById(id);
            if (eventObj == null) return false;

            _cache.Remove(EventsCacheKey);
            return _events.Remove(eventObj);
        }
    }
}