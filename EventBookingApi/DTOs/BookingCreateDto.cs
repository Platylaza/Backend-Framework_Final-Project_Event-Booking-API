using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.DTOs
{
    public class BookingCreateDto
    {
        [Required]
        public int Id;

        [Required]
        public int EventId;

        [Required]
        public int CustomerId;

        [Required]
        [Range(1, 1000)]
        public int NumberOfTickets;
    }
}