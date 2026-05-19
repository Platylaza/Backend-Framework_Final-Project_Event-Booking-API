using EventBookingApi.Models;

namespace EventBookingApi.Services
{
    public interface IEventService
    {
        IEnumerable<Event> GetAll();
        Event? GetById(int id);
        Event Add(Event newEvent);
        bool Update(int id, Event newEvent);
        bool Delete(int id);
    }
}