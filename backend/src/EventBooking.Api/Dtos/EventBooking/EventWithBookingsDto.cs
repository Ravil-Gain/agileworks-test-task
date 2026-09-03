namespace EventBooking.Api.Dtos.EventBooking
{
    public class EventWithBookingsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public List<BookingDto> Bookings { get; set; } = new List<BookingDto>();
    }
}