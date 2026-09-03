using EventBooking.Api.Dtos.Event;
using EventBooking.Api.Interfaces;
using EventBooking.Api.Mappers;
using EventBooking.Api.Types;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.Api.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            var eventDtos = events.Select(e => e.ToEventDto()).ToList();
            return Ok(eventDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent([FromRoute] int id)
        {
            var eventModel = await _eventRepository.GetEventByIdAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            return Ok(eventModel.ToEventDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto eventDto)
        {
            var eventModel = await _eventRepository.CreateEventAsync(eventDto.ToEventModel());

            return Ok(eventModel.ToEventDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventDto eventDto)
        {
            var result = await _eventRepository.UpdateEventAsync(id, eventDto);
            if (result.Status == UpdateEventStatus.NotFound)
            {
                return NotFound();
            }

            if (result.Status == UpdateEventStatus.CapacityConflict)
            {
                return Conflict(new
                {
                    message = "TotalSeats cannot be less than the number of booked seats."
                });
            }

            return Ok(result.Event!.ToEventDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent([FromRoute] int id)
        {
            var eventModel = await _eventRepository.DeleteEventAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            return Ok(eventModel.ToEventDto());
        }
    }
}