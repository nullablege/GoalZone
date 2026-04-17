namespace GoalZone.API.DTOs.StandingDTOs
{
    public class StandingsPageDto
    {
        public string SeasonName { get; set; } = null!;
        public string Subtitle { get; set; } = null!;

        public StandingSummaryDto Summary { get; set; } = new();
        public List<StandingDto> Standings { get; set; } = new();
    }
}
