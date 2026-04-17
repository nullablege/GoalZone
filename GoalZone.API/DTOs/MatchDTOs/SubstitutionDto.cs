namespace GoalZone.API.DTOs.MatchDTOs
{
    public class SubstitutionDto
    {
        public int Minute { get; set; }
        public string TeamSide { get; set; } = null!;
        public string PlayerInName { get; set; } = null!;
        public string PlayerOutName { get; set; } = null!;
    }
}
