using EventBooking.Api.Dtos.EventBooking;
using EventBooking.Api.Models;

namespace EventBooking.Api.Mappers
{
    public static class EventBookingMappers
    {
        public static EventWithBookingsDto ToEventWithBookingsDto(this Event eventModel)
        {
            return new EventWithBookingsDto
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                EventDate = eventModel.EventDate,
                TotalSeats = eventModel.TotalSeats,
                AvailableSeats = eventModel.AvailableSeats,
                Bookings = eventModel.Bookings.Select(b => b.ToBookingDto()).ToList()
            };
        }
    }
}
