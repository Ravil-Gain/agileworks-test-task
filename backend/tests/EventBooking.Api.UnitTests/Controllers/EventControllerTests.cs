using EventBooking.Api.Controllers;
using EventBooking.Api.Dtos.Event;
using EventBooking.Api.Interfaces;
using EventBooking.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventBooking.Api.UnitTests.Controllers;

public sealed class EventControllerTests
{
    private readonly Mock<IEventRepository> _repository = new();
    private readonly EventController _controller;

    public EventControllerTests()
    {
        _controller = new EventController(_repository.Object);
    }

    [Fact]
    public async Task GetEvents_ReturnsOkWithMappedEvents()
    {
        var events = new List<Event>
        {
            CreateEvent(1, "Conference", 100, 75),
            CreateEvent(2, "Workshop", 20, 4)
        };
        _repository.Setup(repository => repository.GetAllEventsAsync()).ReturnsAsync(events);

        var result = await _controller.GetEvents();

        var response = Assert.IsType<OkObjectResult>(result);
        var eventDtos = Assert.IsAssignableFrom<List<EventDto>>(response.Value);
        Assert.Equal(events.Count, eventDtos.Count);
        Assert.Equal(events[0].Title, eventDtos[0].Title);
        Assert.Equal(events[0].AvailableSeats, eventDtos[0].AvailableSeats);
        _repository.Verify(repository => repository.GetAllEventsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetEvent_WhenEventExists_ReturnsOkWithMappedEvent()
    {
        var eventModel = CreateEvent(7, "Conference", 100, 75);
        _repository.Setup(repository => repository.GetEventByIdAsync(eventModel.Id)).ReturnsAsync(eventModel);

        var result = await _controller.GetEvent(eventModel.Id);

        var response = Assert.IsType<OkObjectResult>(result);
        var eventDto = Assert.IsType<EventDto>(response.Value);
        Assert.Equal(eventModel.Id, eventDto.Id);
        Assert.Equal(eventModel.Title, eventDto.Title);
        Assert.Equal(eventModel.AvailableSeats, eventDto.AvailableSeats);
        _repository.Verify(repository => repository.GetEventByIdAsync(eventModel.Id), Times.Once);
    }

    [Fact]
    public async Task GetEvent_WhenEventDoesNotExist_ReturnsNotFound()
    {
        _repository.Setup(repository => repository.GetEventByIdAsync(404)).ReturnsAsync((Event?)null);

        var result = await _controller.GetEvent(404);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateEvent_ReturnsOkWithCreatedEventAndMapsInput()
    {
        var input = new CreateEventDto
        {
            Title = "Conference",
            Description = "A technical conference",
            EventDate = new DateTime(2026, 10, 1),
            TotalSeats = 100
        };
        var createdEvent = CreateEvent(10, input.Title, input.TotalSeats, input.TotalSeats);
        Event? submittedEvent = null;
        _repository
            .Setup(repository => repository.CreateEventAsync(It.IsAny<Event>()))
            .Callback<Event>(eventModel => submittedEvent = eventModel)
            .ReturnsAsync(createdEvent);

        var result = await _controller.CreateEvent(input);

        var response = Assert.IsType<OkObjectResult>(result);
        var eventDto = Assert.IsType<EventDto>(response.Value);
        Assert.Equal(createdEvent.Id, eventDto.Id);
        Assert.Equal(input.Title, submittedEvent!.Title);
        Assert.Equal(input.Description, submittedEvent.Description);
        Assert.Equal(input.EventDate, submittedEvent.EventDate);
        Assert.Equal(input.TotalSeats, submittedEvent.TotalSeats);
        Assert.Equal(input.TotalSeats, submittedEvent.AvailableSeats);
        _repository.Verify(repository => repository.CreateEventAsync(It.IsAny<Event>()), Times.Once);
    }

    [Fact]
    public async Task UpdateEvent_WhenUpdated_ReturnsOkWithMappedEvent()
    {
        var input = new UpdateEventDto { Title = "Updated", Description = "Updated description", TotalSeats = 80 };
        var updatedEvent = CreateEvent(3, input.Title, input.TotalSeats, 60);
        _repository.Setup(repository => repository.UpdateEventAsync(3, input))
            .ReturnsAsync(new UpdateEventResult(UpdateEventStatus.Updated, updatedEvent));

        var result = await _controller.UpdateEvent(3, input);

        var response = Assert.IsType<OkObjectResult>(result);
        var eventDto = Assert.IsType<EventDto>(response.Value);
        Assert.Equal(updatedEvent.Id, eventDto.Id);
        Assert.Equal(updatedEvent.Title, eventDto.Title);
        _repository.Verify(repository => repository.UpdateEventAsync(3, input), Times.Once);
    }

    [Theory]
    [InlineData(UpdateEventStatus.NotFound)]
    [InlineData(UpdateEventStatus.CapacityConflict)]
    public async Task UpdateEvent_WhenUpdateFails_ReturnsExpectedError(UpdateEventStatus status)
    {
        var input = new UpdateEventDto { Title = "Updated", Description = "Updated description", TotalSeats = 1 };
        _repository.Setup(repository => repository.UpdateEventAsync(3, input))
            .ReturnsAsync(new UpdateEventResult(status));

        var result = await _controller.UpdateEvent(3, input);

        if (status == UpdateEventStatus.NotFound)
        {
            Assert.IsType<NotFoundResult>(result);
        }
        else
        {
            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(
                "TotalSeats cannot be less than the number of booked seats.",
                conflict.Value?.GetType().GetProperty("message")?.GetValue(conflict.Value));
        }
    }

    [Fact]
    public async Task DeleteEvent_WhenEventExists_ReturnsOkWithMappedEvent()
    {
        var eventModel = CreateEvent(8, "Conference", 100, 99);
        _repository.Setup(repository => repository.DeleteEventAsync(eventModel.Id)).ReturnsAsync(eventModel);

        var result = await _controller.DeleteEvent(eventModel.Id);

        var response = Assert.IsType<OkObjectResult>(result);
        var eventDto = Assert.IsType<EventDto>(response.Value);
        Assert.Equal(eventModel.Id, eventDto.Id);
        Assert.Equal(eventModel.AvailableSeats, eventDto.AvailableSeats);
    }

    [Fact]
    public async Task DeleteEvent_WhenEventDoesNotExist_ReturnsNotFound()
    {
        _repository.Setup(repository => repository.DeleteEventAsync(404)).ReturnsAsync((Event?)null);

        var result = await _controller.DeleteEvent(404);

        Assert.IsType<NotFoundResult>(result);
    }

    private static Event CreateEvent(int id, string title, int totalSeats, int availableSeats)
    {
        return new Event
        {
            Id = id,
            Title = title,
            Description = $"{title} description",
            EventDate = new DateTime(2026, 10, 1),
            TotalSeats = totalSeats,
            AvailableSeats = availableSeats
        };
    }
}
