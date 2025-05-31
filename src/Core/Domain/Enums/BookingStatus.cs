namespace Domain.Enums;

public enum BookingStatus
{
    PendingApproval, // Oczekuje na zatwierdzenie
    Confirmed,       // Potwierdzona
    CancelledByUser, // Anulowana przez użytkownika
    CancelledByAdmin // Anulowana przez administratora
    // Można dodać inne statusy, np. InProgress, Completed
}