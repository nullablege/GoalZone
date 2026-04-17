namespace GoalZone.API.DTOs.MatchDTOs
{
    public class MatchEventDto
    {
        public int Minute { get; set; }
        public string TeamSide { get; set; } = null!; // Home - Away
        public string PlayerName { get; set; } = null!;
        public string EventType { get; set; } = null!;
        public string? Description { get; set; }
    }
}
