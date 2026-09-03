using System.ComponentModel.DataAnnotations;

namespace EventBooking.Api.Dtos.Event
{
    public class CreateEventDto : IEventDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalSeats { get; set; }
    }
}