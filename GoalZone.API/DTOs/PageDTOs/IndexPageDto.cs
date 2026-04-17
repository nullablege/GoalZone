using GoalZone.API.DTOs.MatchDTOs;

namespace GoalZone.API.DTOs.PageDTOs
{
    public class IndexPageDto
    {
        public int WeekNumber { get; set; }
        public string DateRangeText { get; set; } = null!;
        public int TotalMatchCount { get; set; }
        public int LiveMatchCount { get; set; }
        public int FinishedMatchCount { get; set; }
        public int UpcomingMatchCount { get; set; }

        public ResultFeatureMatchDto? FeaturedMatch { get; set; }

        public List<ResultMatchDto> LiveMatches { get; set; } = new();
        public List<ResultMatchDto> FinishedMatches { get; set; } = new();
        public List<ResultMatchDto> UpcomingMatches { get; set; } = new();
    }
}
