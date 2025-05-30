using System;
using System.ComponentModel.DataAnnotations;

namespace SystemRezerwacji.WebApp.Models
{
    public class BookingRequestDto
    {
        [Required]
        public Guid ResourceId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? Notes { get; set; }
    }
}