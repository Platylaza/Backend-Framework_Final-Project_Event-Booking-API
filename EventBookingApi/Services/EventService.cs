using EventBookingApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EventBookingApi.Services
{
    public class EventService: IEventService
    {
        private readonly IMemoryCache _cache;
    }
}