using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.DTOs
{
    public class EventCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name = string.Empty;

        [Required]
        public DateOnly Date;

        [Required]
        [StringLength(100)]
        public string Location = string.Empty;

        [Required]
        [Range(1, 1000)]
        public int Capacity {get; set;}
    }
}