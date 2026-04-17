namespace GoalZone.API.DTOs.StandingDTOs
{
    public class StandingSummaryDto
    {
        public string LeaderTeamName { get; set; } = null!;
        public int LeaderPoints { get; set; }

        public string MostGoalsTeamName { get; set; } = null!;
        public int MostGoals { get; set; }

        public string BestDefenseTeamName { get; set; } = null!;
        public int LeastConceded { get; set; }

        public string MatchOfTheWeekText { get; set; } = null!;
    }
}
