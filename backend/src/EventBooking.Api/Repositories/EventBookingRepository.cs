using EventBooking.Api.Data;
using EventBooking.Api.Interfaces;
using EventBooking.Api.Models;
using EventBooking.Api.Types;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.Api.Repositories
{
    public class EventBookingRepository(EventBookingDBContext context) : IEventBookingRepository
    {
        private readonly EventBookingDBContext _context = context;

        public async Task<CreateEventBookingResult> CreateEventBookingAsync(int eventId, Booking booking)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (!await _context.Events.AnyAsync(e => e.Id == eventId))
            {
                return new(CreateEventBookingStatus.NotFound);
            }

            // Atomically decrement only when a seat is available. Concurrent requests
            // cannot decrement the value below zero.
            var seatsUpdated = await _context.Events
                .Where(e => e.Id == eventId && e.AvailableSeats > 0)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(e => e.AvailableSeats, e => e.AvailableSeats - 1));

            if (seatsUpdated == 0)
            {
                return new(CreateEventBookingStatus.CapacityConflict);
            }

            booking.EventId = eventId;
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new(CreateEventBookingStatus.Created, booking);
        }

        public async Task<Event?> GetEventWithBookingsByIdAsync(int eventId)
        {
            var eventModel = await _context.Events.FirstOrDefaultAsync(e => e.Id == eventId);
            if (eventModel == null)
            {
                return null;
            }
            var eventWithBookings = await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            return eventWithBookings;
        }

    }
}