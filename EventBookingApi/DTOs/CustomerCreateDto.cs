using System.ComponentModel.DataAnnotations;

namespace EventBookingApi.DTOs
{
    public class CustomerCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}