namespace GoalZone.API.DTOs.MatchDTOs
{
    public class FixtureMatchDto
    {
        public int Id { get; set; }

        public string HomeTeamName { get; set; } = null!;
        public string AwayTeamName { get; set; } = null!;

        public string? HomeTeamLogoUrl { get; set; }
        public string? AwayTeamLogoUrl { get; set; }

        public DateTime MatchDate { get; set; }
        public string StadiumName { get; set; } = null!;

        public int WeekNumber { get; set; }
        public bool IsFeaturedMatch { get; set; }

        public string MatchTime => MatchDate.ToString("HH:mm");

        public List<string> HomeLast5Matches { get; set; } = new();
        public List<string> AwayLast5Matches { get; set; } = new();
    }
}
