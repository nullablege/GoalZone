using GoalZone.API.Enums;

namespace GoalZone.API.DTOs.AdminDTOs
{
    public class AddMatchDto
    {
        public int SeasonId { get; set; }

        public int WeekNumber { get; set; }

        public DateTime MatchDate { get; set; }

        public int HomeTeamId { get; set; }

        public int AwayTeamId { get; set; }


        public string StadiumName { get; set; } = null!;

        public MatchStatus Status { get; set; } = MatchStatus.NotStarted;

        public bool IsFeaturedMatch { get; set; }

        public int HomeHalfTimeScore { get; set; }

        public int AwayHalfTimeScore { get; set; }

        public int HomeFullTimeScore { get; set; }

        public int AwayFullTimeScore { get; set; }
    }
}
