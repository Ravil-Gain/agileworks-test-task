using EventBooking.Api.Models;
using EventBooking.Api.Types;

namespace EventBooking.Api.Interfaces
{
    public interface IEventBookingRepository
    {
        Task<CreateEventBookingResult> CreateEventBookingAsync(int eventId,Booking booking);
        Task<Event?> GetEventWithBookingsByIdAsync(int eventId);
    }
}