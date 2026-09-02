
using EventBooking.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EventBooking.Api.Data
{
    public class EventBookingDBContext(DbContextOptions<EventBookingDBContext> options) : DbContext(options)
    {
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    }
}