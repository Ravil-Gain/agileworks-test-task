using EventBooking.Api.Interfaces;
using EventBooking.Api.Mappers;
using EventBooking.Api.Models;
using EventBooking.Api.Types;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.Api.Controllers
{
    [Route("api/bookings")]
    public class EventBookingController : ControllerBase
    {
        private readonly IEventBookingRepository _eventBookingRepository;

        public EventBookingController(IEventBookingRepository eventBookingRepository)
        {
            _eventBookingRepository = eventBookingRepository;
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventWithBookings(int eventId)
        {
            var eventModel = await _eventBookingRepository.GetEventWithBookingsByIdAsync(eventId);
            if (eventModel == null)
            {
                return NotFound();
            }

            return Ok(eventModel.ToEventWithBookingsDto());
        }

        [HttpPost("{eventId}")]
        public async Task<IActionResult> CreateEventBooking(int eventId, [FromBody] Booking booking)
        {
            var result = await _eventBookingRepository.CreateEventBookingAsync(eventId, booking);
            if (result.status == CreateEventBookingStatus.NotFound)
            {
                return NotFound();
            }

            if (result.status == CreateEventBookingStatus.CapacityConflict)
            {
                return Conflict(new
                {
                    message = "Event capacity has been reached. No available seats."
                });
            }

            return Ok(result.booking!.ToBookingDto());
        }
    }
}