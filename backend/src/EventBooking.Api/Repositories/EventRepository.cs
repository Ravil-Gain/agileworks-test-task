using EventBooking.Api.Data;
using EventBooking.Api.Dtos.Event;
using EventBooking.Api.Interfaces;
using EventBooking.Api.Models;
using EventBooking.Api.Types;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.Api.Repositories
{
    public class EventRepository(EventBookingDBContext context) : IEventRepository
    {
        private readonly EventBookingDBContext _context = context;

        public async Task<Event> CreateEventAsync(Event eventModel)
        {
            await _context.Events.AddAsync(eventModel);
            await _context.SaveChangesAsync();
            return eventModel;
        }

        public async Task<Event?> DeleteEventAsync(int id)
        {
            var eventModel = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventModel == null)
            {
                return null;
            }

            _context.Events.Remove(eventModel);
            await _context.SaveChangesAsync();
            return eventModel;
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            // Use AsNoTracking for better performance when querying data that won't be modified
            return await _context.Events.AsNoTracking().ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<UpdateEventResult> UpdateEventAsync(int id, UpdateEventDto eventDto)
        {
            var eventModel = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventModel == null)
            {
                return new(UpdateEventStatus.NotFound);
            }

            var bookedSeats = eventModel.TotalSeats - eventModel.AvailableSeats;
            if (eventDto.TotalSeats < bookedSeats)
            {
                return new(UpdateEventStatus.CapacityConflict);
            }

            eventModel.Title = eventDto.Title;
            eventModel.Description = eventDto.Description;
            eventModel.EventDate = eventDto.EventDate;
            eventModel.TotalSeats = eventDto.TotalSeats;
            eventModel.AvailableSeats = eventDto.TotalSeats - bookedSeats;

            await _context.SaveChangesAsync();
            return new(UpdateEventStatus.Updated, eventModel);
        }
    }
}