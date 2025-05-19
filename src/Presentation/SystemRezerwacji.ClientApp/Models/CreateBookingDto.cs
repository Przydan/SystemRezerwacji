using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SystemRezerwacji.ClientApp.Models
{
    public class CreateBookingDto : IValidatableObject
    {
        [Required(ErrorMessage = "ResourceId is required")]
        public Guid ResourceId { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public DateTime? StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public DateTime? EndTime { get; set; }

        public string? Notes { get; set; }

        // Cross-field validation to ensure EndTime is after StartTime
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime.HasValue && EndTime.HasValue && EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "EndTime must be later than StartTime.",
                    new[] { nameof(EndTime) }
                );
            }
        }
    }

    
}