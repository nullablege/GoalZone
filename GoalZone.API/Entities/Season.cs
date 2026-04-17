using System.Text.RegularExpressions;

namespace GoalZone.API.Entities
{
    public class Season
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!; // 2024/2025

        public List<Match> Matches { get; set; } = new List<Match>();
        public List<StandingCorrection> StandingCorrections { get; set; } = new List<StandingCorrection>();
    }
}
