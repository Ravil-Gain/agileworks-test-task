using EventBooking.Api.Dtos.Event;
using EventBooking.Api.Models;

namespace EventBooking.Api.Mappers
{
    public static class EventMappers
    {
        public static EventDto ToEventDto(this Event eventModel)
        {
            return new EventDto
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                EventDate = eventModel.EventDate,
                TotalSeats = eventModel.TotalSeats,
                AvailableSeats = eventModel.AvailableSeats
            };
        }

        public static Event ToEventModel(this IEventDto eventDto)
        {
            return new Event
            {
                Title = eventDto.Title,
                Description = eventDto.Description,
                EventDate = eventDto.EventDate,
                TotalSeats = eventDto.TotalSeats,
                AvailableSeats = eventDto.TotalSeats
            };
        }
    }
}