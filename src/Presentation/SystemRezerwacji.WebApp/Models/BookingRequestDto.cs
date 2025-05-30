using System;

namespace SystemRezerwacji.WebApp.Models
{
    /// <summary>
    /// DTO służące zarówno do tworzenia, jak i edycji rezerwacji.
    /// </summary>
    public class BookingRequestDto
    {
        /// <summary>
        /// Id istniejącej rezerwacji przy edycji; Guid.Empty przy tworzeniu nowej.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Id zasobu, który rezerwujemy.
        /// </summary>
        public Guid ResourceId { get; set; }

        /// <summary>
        /// Czas rozpoczęcia rezerwacji.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Czas zakończenia rezerwacji.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Opcjonalne notatki do rezerwacji.
        /// </summary>
        public string? Notes { get; set; }
    }
}