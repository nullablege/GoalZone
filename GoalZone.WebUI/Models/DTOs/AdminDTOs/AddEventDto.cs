using GoalZone.API.Enums;

namespace GoalZone.API.DTOs.AdminDTOs
{
    public class AddEventDto
    {
        public int MatchId { get; set; }

        public int TeamId { get; set; }

        public int PlayerId { get; set; }

        public MatchEventType EventType { get; set; }

        public int Minute { get; set; }

        public string? Description { get; set; }
    }
}
