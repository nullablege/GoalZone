using GoalZone.API.Enums;

namespace GoalZone.API.Entities
{
    public class Match
    {
        public int Id { get; set; }

        public int SeasonId { get; set; }
        public Season Season { get; set; } = null!;

        public int WeekNumber { get; set; }

        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;

        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;

        public DateTime MatchDate { get; set; }
        public string StadiumName { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public MatchStatus Status { get; set; }

        public int HomeHalfTimeScore { get; set; }
        public int AwayHalfTimeScore { get; set; }

        public int HomeFullTimeScore { get; set; }
        public int AwayFullTimeScore { get; set; }

        public bool IsFeaturedMatch { get; set; }

        public MatchStatistic? Statistic { get; set; }
        public MatchInfo? Info { get; set; }

        public List<MatchEvent> Events { get; set; } = new List<MatchEvent>();
        public List<MatchSubstitution> Substitutions { get; set; } = new List<MatchSubstitution>();
    }
}
