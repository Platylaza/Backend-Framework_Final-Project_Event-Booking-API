namespace EventBookingApi.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int CustomerId { get; set; }
        public int NumberOfTickets {get; set;}
        public BookingError Error {get; set;} = null;
    }

    public class BookingError 
    {
        public string Message { get; set; }
        public int NumberOfTicketsAvailable { get; set; }
    }
}