using GoalZone.API.DTOs.MatchDTOs;

namespace GoalZone.API.DTOs.PageDTOs
{
    public class FixturesPageDto
    {
        public int WeekNumber { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int MatchCount { get; set; }
        public int TopMatchCount { get; set; }

        public int PreviousWeekNumber => WeekNumber - 1;
        public int NextWeekNumber => WeekNumber + 1;

        public string StartMatchDay => StartDate.ToString("ddd", new System.Globalization.CultureInfo("tr-TR"));
        public string EndMatchDay => EndDate.ToString("ddd", new System.Globalization.CultureInfo("tr-TR"));

        public string FullStartDate => StartDate.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("tr-TR"));
        public string FullEndDate => EndDate.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("tr-TR"));

        public string TimeToFirstMatch =>
            DateTime.Now > StartDate
                ? "0"
                : (StartDate - DateTime.Now).ToString(@"dd\.hh\:mm\:ss");

        public List<FixtureDayGroupDto> DayGroups { get; set; } = new();
    }
}
