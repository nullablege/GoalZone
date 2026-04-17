namespace GoalZone.API.DTOs.MatchDTOs
{
    public class CardDto
    {
        public int Minute { get; set; }
        public string TeamSide { get; set; } = null!;
        public string PlayerName { get; set; } = null!;
        public string CardType { get; set; } = null!; // Yellow - Red
    }
}
