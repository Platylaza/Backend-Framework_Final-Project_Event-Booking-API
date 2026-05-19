using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.DTOs
{
    public class EventCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name {get; set;} = string.Empty;

        [Required]
        public DateOnly Date {get; set;}

        [Required]
        [StringLength(100)]
        public string Location {get; set;} = string.Empty;

        [Required]
        [Range(1, 1000)]
        public int Capacity {get; set;}
    }
}