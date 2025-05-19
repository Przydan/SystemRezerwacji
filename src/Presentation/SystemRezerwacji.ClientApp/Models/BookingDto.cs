using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SystemRezerwacji.ClientApp.Models
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

