using EventBooking.Api.Dtos;
using EventBooking.Api.Models;

namespace EventBooking.Api.Mappers
{
    public static class BookingMappers
    {
        public static BookingDto ToBookingDto(this Booking booking)
        {
            return new BookingDto
            {
                FirstName = booking.FirstName,
                LastName = booking.LastName
            };
        }
    }
}