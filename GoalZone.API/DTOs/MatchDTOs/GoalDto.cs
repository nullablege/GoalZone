namespace GoalZone.API.DTOs.MatchDTOs
{
    public class GoalDto
    {
        public int Minute { get; set; }
        public string TeamSide { get; set; } = null!;
        public string PlayerName { get; set; } = null!;
    }
}
