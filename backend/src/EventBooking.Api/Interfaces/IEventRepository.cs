using EventBooking.Api.Dtos.Event;
using EventBooking.Api.Models;

namespace EventBooking.Api.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int id);
        Task<Event> CreateEventAsync(Event eventModel);
        Task<UpdateEventResult> UpdateEventAsync(int id, UpdateEventDto eventDto);
        Task<Event?> DeleteEventAsync(int id);
    }
}