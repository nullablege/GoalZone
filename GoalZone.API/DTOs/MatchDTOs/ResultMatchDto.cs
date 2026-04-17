namespace GoalZone.API.DTOs.MatchDTOs
{
    public class ResultMatchDto
    {
        public int Id { get; set; }
        public string HomeTeamName { get; set; } = null!;
        public string AwayTeamName { get; set; } = null!;
        public string? HomeTeamLogoUrl { get; set; }
        public string? AwayTeamLogoUrl { get; set; }
        public int HomeFullTimeScore { get; set; }
        public int AwayFullTimeScore { get; set; }
        public DateTime MatchDate { get; set; }
        public string StadiumName { get; set; } = null!;
    }
}
