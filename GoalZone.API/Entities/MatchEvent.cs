using GoalZone.API.Enums;

namespace GoalZone.API.Entities
{
    public class MatchEvent
    {
        public int Id { get; set; }

        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        public MatchEventType EventType { get; set; }
        public int Minute { get; set; }

        public string? Description { get; set; }
    }
}
