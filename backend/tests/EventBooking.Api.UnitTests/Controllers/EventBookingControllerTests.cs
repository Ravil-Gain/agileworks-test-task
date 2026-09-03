using EventBooking.Api.Controllers;
using EventBooking.Api.Dtos;
using EventBooking.Api.Dtos.EventBooking;
using EventBooking.Api.Interfaces;
using EventBooking.Api.Models;
using EventBooking.Api.Types;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventBooking.Api.UnitTests.Controllers;

public sealed class EventBookingControllerTests
{
    private readonly Mock<IEventBookingRepository> _repository = new();
    private readonly EventBookingController _controller;

    public EventBookingControllerTests()
    {
        _controller = new EventBookingController(_repository.Object);
    }

    [Fact]
    public async Task GetEventWithBookings_WhenEventExists_ReturnsOkWithMappedEvent()
    {
        var eventModel = CreateEvent(7);
        eventModel.Bookings.Add(new Booking
        {
            Id = 11,
            EventId = eventModel.Id,
            FirstName = "Jane",
            LastName = "Smith"
        });
        _repository
            .Setup(repository => repository.GetEventWithBookingsByIdAsync(eventModel.Id))
            .ReturnsAsync(eventModel);

        var result = await _controller.GetEventWithBookings(eventModel.Id);

        var response = Assert.IsType<OkObjectResult>(result);
        var eventDto = Assert.IsType<EventWithBookingsDto>(response.Value);
        Assert.Equal(eventModel.Id, eventDto.Id);
        Assert.Equal(eventModel.Title, eventDto.Title);
        Assert.Equal(eventModel.AvailableSeats, eventDto.AvailableSeats);
        var bookingDto = Assert.Single(eventDto.Bookings);
        Assert.Equal("Jane", bookingDto.FirstName);
        Assert.Equal("Smith", bookingDto.LastName);
        _repository.Verify(repository => repository.GetEventWithBookingsByIdAsync(eventModel.Id), Times.Once);
    }

    [Fact]
    public async Task GetEventWithBookings_WhenEventDoesNotExist_ReturnsNotFound()
    {
        _repository
            .Setup(repository => repository.GetEventWithBookingsByIdAsync(404))
            .ReturnsAsync((Event?)null);

        var result = await _controller.GetEventWithBookings(404);

        Assert.IsType<NotFoundResult>(result);
        _repository.Verify(repository => repository.GetEventWithBookingsByIdAsync(404), Times.Once);
    }

    [Fact]
    public async Task CreateEventBooking_WhenBookingIsCreated_ReturnsOkWithMappedBooking()
    {
        var booking = new Booking
        {
            FirstName = "Grace",
            LastName = "Hopper"
        };
        var createdBooking = new Booking
        {
            Id = 12,
            EventId = 5,
            FirstName = booking.FirstName,
            LastName = booking.LastName
        };
        _repository
            .Setup(repository => repository.CreateEventBookingAsync(5, booking))
            .ReturnsAsync(new CreateEventBookingResult(CreateEventBookingStatus.Created, createdBooking));

        var result = await _controller.CreateEventBooking(5, booking);

        var response = Assert.IsType<OkObjectResult>(result);
        var bookingDto = Assert.IsType<BookingDto>(response.Value);
        Assert.Equal(createdBooking.FirstName, bookingDto.FirstName);
        Assert.Equal(createdBooking.LastName, bookingDto.LastName);
        _repository.Verify(repository => repository.CreateEventBookingAsync(5, booking), Times.Once);
    }

    [Fact]
    public async Task CreateEventBooking_WhenEventDoesNotExist_ReturnsNotFound()
    {
        var booking = new Booking { FirstName = "Grace", LastName = "Hopper" };
        _repository
            .Setup(repository => repository.CreateEventBookingAsync(404, booking))
            .ReturnsAsync(new CreateEventBookingResult(CreateEventBookingStatus.NotFound));

        var result = await _controller.CreateEventBooking(404, booking);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateEventBooking_WhenCapacityIsReached_ReturnsConflictWithMessage()
    {
        var booking = new Booking { FirstName = "Grace", LastName = "Hopper" };
        _repository
            .Setup(repository => repository.CreateEventBookingAsync(5, booking))
            .ReturnsAsync(new CreateEventBookingResult(CreateEventBookingStatus.CapacityConflict));

        var result = await _controller.CreateEventBooking(5, booking);

        var response = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(
            "Event capacity has been reached. No available seats.",
            response.Value?.GetType().GetProperty("message")?.GetValue(response.Value));
    }

    private static Event CreateEvent(int id)
    {
        return new Event
        {
            Id = id,
            Title = "Conference",
            Description = "A technical conference",
            EventDate = new DateTime(2026, 10, 1),
            TotalSeats = 100,
            AvailableSeats = 75
        };
    }
}
