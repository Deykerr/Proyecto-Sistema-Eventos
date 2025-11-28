namespace Sistema_Eventos.Models
{
    public enum UserRole
    {
        Admin,
        Organizer,
        User
    }

    public enum EventStatus
    {
        Draft,      // Borrador
        Published,  // Publicado
        Canceled    // Cancelado
    }

    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Canceled,
        Expired
    }

    public enum NotificationType
    {
        Email,
        SMS,
        Push
    }

    public enum NotificationStatus
    {
        Pending,
        Sent,
        Failed
    }
}