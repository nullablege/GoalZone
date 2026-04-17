namespace GoalZone.API.DTOs.MatchDTOs
{
    public class MatchDetailDto
    {
        public int Id { get; set; }

        public string HomeTeamName { get; set; } = null!;
        public string AwayTeamName { get; set; } = null!;

        public string? HomeTeamLogoUrl { get; set; }
        public string? AwayTeamLogoUrl { get; set; }

        public int HomeHalfTimeScore { get; set; }
        public int AwayHalfTimeScore { get; set; }

        public int HomeFullTimeScore { get; set; }
        public int AwayFullTimeScore { get; set; }

        public string Status { get; set; } = null!;

        public DateTime MatchDate { get; set; }
        public string StadiumName { get; set; } = null!;
        public int WeekNumber { get; set; }
        public string SeasonName { get; set; } = null!;

        public string? RefereeName { get; set; }
        public int? Attendance { get; set; }

        public MatchStatisticDto? Statistics { get; set; }

        public List<MatchEventDto> Timeline { get; set; } = new();
        public List<GoalDto> Goals { get; set; } = new();
        public List<CardDto> Cards { get; set; } = new();
        public List<SubstitutionDto> Substitutions { get; set; } = new();
    }
}
