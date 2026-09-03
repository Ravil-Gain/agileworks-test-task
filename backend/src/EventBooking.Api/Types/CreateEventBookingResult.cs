using EventBooking.Api.Models;

namespace EventBooking.Api.Types
{
    public enum CreateEventBookingStatus
    {
        Created,
        NotFound,
        CapacityConflict
    }
    public sealed record CreateEventBookingResult(CreateEventBookingStatus status, Booking? booking = null);

}