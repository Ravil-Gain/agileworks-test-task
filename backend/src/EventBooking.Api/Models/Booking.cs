namespace EventBooking.Api.Models
{
    public sealed class Booking: BaseEntity
    {
        public int? EventId { get; set; }
        public Event? Event { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}