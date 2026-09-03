namespace EventBooking.Api.Dtos.Event
{
    public interface IEventDto
    {
        string Title { get; }
        string Description { get; }
        DateTime EventDate { get; }
        int TotalSeats { get; }
    }
}
