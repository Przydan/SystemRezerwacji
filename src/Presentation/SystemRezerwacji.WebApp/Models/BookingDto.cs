namespace SystemRezerwacji.WebApp.Models
{
    public class BookingDto
    {
        public Guid   Id           { get; set; }
        public Guid   ResourceId   { get; set; }
        public string ResourceName { get; set; } = string.Empty; // nazwa zasobu
        public DateTime StartTime  { get; set; }
        public DateTime EndTime    { get; set; }
        public string Status       { get; set; } = string.Empty; // np. "Confirmed", "Cancelled"
        public string UserName     { get; set; } = string.Empty; // imię/nazwa użytkownika
        public string Notes        { get; set; } = string.Empty; // dodatkowe uwagi
    }
}