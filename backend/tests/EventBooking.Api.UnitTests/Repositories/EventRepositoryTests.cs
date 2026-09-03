using EventBooking.Api.Data;
using EventBooking.Api.Dtos.Event;
using EventBooking.Api.Models;
using EventBooking.Api.Repositories;
using EventBooking.Api.Types;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EventBooking.Api.UnitTests.Repositories;

public sealed class EventRepositoryTests
{
    [Fact]
    public async Task GetAllEventsAsync_ReturnsAllEvents()
    {
        await using var context = CreateContext();
        context.Events.AddRange(CreateEvent(1, "First"), CreateEvent(2, "Second"));
        await context.SaveChangesAsync();
        var repository = new EventRepository(context);

        var result = await repository.GetAllEventsAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, eventModel => eventModel.Title == "First");
        Assert.Contains(result, eventModel => eventModel.Title == "Second");
    }

    [Fact]
    public async Task GetEventByIdAsync_ReturnsMatchingEvent()
    {
        await using var context = CreateContext();
        context.Events.AddRange(CreateEvent(1, "First"), CreateEvent(2, "Second"));
        await context.SaveChangesAsync();
        var repository = new EventRepository(context);

        var result = await repository.GetEventByIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal("Second", result.Title);
    }

    [Fact]
    public async Task GetEventByIdAsync_WhenEventDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext();
        var repository = new EventRepository(context);

        var result = await repository.GetEventByIdAsync(404);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateEventAsync_PersistsAndReturnsEvent()
    {
        await using var context = CreateContext();
        var repository = new EventRepository(context);
        var eventModel = CreateEvent(1, "New event");

        var result = await repository.CreateEventAsync(eventModel);

        Assert.Same(eventModel, result);
        Assert.Equal(eventModel, await context.Events.SingleAsync());
    }

    [Fact]
    public async Task UpdateEventAsync_UpdatesEventAndPreservesBookedSeats()
    {
        await using var context = CreateContext();
        context.Events.Add(CreateEvent(1, "Original", totalSeats: 100, availableSeats: 70));
        await context.SaveChangesAsync();
        var repository = new EventRepository(context);
        var update = new UpdateEventDto
        {
            Title = "Updated",
            Description = "Updated description",
            EventDate = new DateTime(2026, 12, 1),
            TotalSeats = 80
        };

        var result = await repository.UpdateEventAsync(1, update);

        Assert.Equal(UpdateEventStatus.Updated, result.Status);
        Assert.NotNull(result.Event);
        Assert.Equal("Updated", result.Event.Title);
        Assert.Equal("Updated description", result.Event.Description);
        Assert.Equal(80, result.Event.TotalSeats);
        Assert.Equal(50, result.Event.AvailableSeats);
    }

    [Fact]
    public async Task UpdateEventAsync_WhenEventDoesNotExist_ReturnsNotFound()
    {
        await using var context = CreateContext();
        var repository = new EventRepository(context);

        var result = await repository.UpdateEventAsync(404, new UpdateEventDto { TotalSeats = 10 });

        Assert.Equal(UpdateEventStatus.NotFound, result.Status);
        Assert.Null(result.Event);
    }

    [Fact]
    public async Task UpdateEventAsync_WhenCapacityIsBelowBookedSeats_ReturnsConflictWithoutUpdating()
    {
        await using var context = CreateContext();
        context.Events.Add(CreateEvent(1, "Original", totalSeats: 100, availableSeats: 70));
        await context.SaveChangesAsync();
        var repository = new EventRepository(context);
        var update = new UpdateEventDto { Title = "Should not update", TotalSeats = 29 };

        var result = await repository.UpdateEventAsync(1, update);

        Assert.Equal(UpdateEventStatus.CapacityConflict, result.Status);
        var unchanged = await context.Events.SingleAsync();
        Assert.Equal("Original", unchanged.Title);
        Assert.Equal(100, unchanged.TotalSeats);
        Assert.Equal(70, unchanged.AvailableSeats);
    }

    [Fact]
    public async Task DeleteEventAsync_WhenEventExists_RemovesAndReturnsEvent()
    {
        await using var context = CreateContext();
        context.Events.Add(CreateEvent(1, "To delete"));
        await context.SaveChangesAsync();
        var repository = new EventRepository(context);

        var result = await repository.DeleteEventAsync(1);

        Assert.NotNull(result);
        Assert.Equal("To delete", result.Title);
        Assert.Empty(await context.Events.ToListAsync());
    }

    [Fact]
    public async Task DeleteEventAsync_WhenEventDoesNotExist_ReturnsNull()
    {
        await using var context = CreateContext();
        var repository = new EventRepository(context);

        var result = await repository.DeleteEventAsync(404);

        Assert.Null(result);
    }

    private static EventBookingDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<EventBookingDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventBookingDBContext(options);
    }

    private static Event CreateEvent(
        int id,
        string title,
        int totalSeats = 100,
        int availableSeats = 100)
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
