namespace GoalZone.API.Entities
{
    public class MatchInfo
    {
        public int Id { get; set; }

        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;

        public string? RefereeName { get; set; }
        public int? Attendance { get; set; }
    }
}
