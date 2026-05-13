using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.DTOs
{
    public class BookingCreateDto
    {
        [Required]
        public int EventId {get; set;}

        [Required]
        public int CustomerId {get; set;}

        [Required]
        [Range(1, 1000)]
        public int NumberOfTickets {get; set;}
    }
}