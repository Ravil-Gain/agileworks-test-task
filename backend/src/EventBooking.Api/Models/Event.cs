namespace EventBooking.Api.Models
{
    public sealed class Event: BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}