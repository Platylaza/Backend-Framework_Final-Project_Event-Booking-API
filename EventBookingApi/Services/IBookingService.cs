using EventBookingApi.Models;

namespace EventBookingApi.Services
{
    public interface IBookingService
    {
        IEnumerable<Booking> GetAll();
        Booking? GetById(int id);
        IEnumerable<Booking>? GetByEventId(int id);
        IEnumerable<Booking>? GetByCustomerId(int id);
        Booking Add(Booking newBooking);
        bool Update(int id, Booking newBooking);
        bool Delete(int id);
    }
}