namespace EventBookingApi.DTOs
{
    public class BookingReadDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int CustomerId { get; set; }
        public int NumberOfTickets {get; set;}
    }
}