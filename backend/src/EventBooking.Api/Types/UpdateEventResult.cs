using EventBooking.Api.Models;

namespace EventBooking.Api.Types
{
    public enum UpdateEventStatus
    {
        Updated,
        NotFound,
        CapacityConflict
    }

    public sealed record UpdateEventResult(UpdateEventStatus Status, Event? Event = null);
}
