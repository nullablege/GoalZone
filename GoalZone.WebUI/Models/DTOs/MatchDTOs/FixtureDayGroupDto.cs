namespace GoalZone.API.DTOs.MatchDTOs
{
    public class FixtureDayGroupDto
    {
        public string DayName { get; set; } = null!;
        public DateTime Date { get; set; }
        public int MatchCount { get; set; }

        public List<FixtureMatchDto> Matches { get; set; } = new();
    }
}
