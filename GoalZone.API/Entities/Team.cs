using System.Numerics;
using System.Text.RegularExpressions;

namespace GoalZone.API.Entities
{
    public class Team
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string ShortName { get; set; } = null!;
        public string? LogoUrl { get; set; }

        public List<Player> Players { get; set; } = new List<Player>();

        public List<Match> HomeMatches { get; set; } = new List<Match>();
        public List<Match> AwayMatches { get; set; } = new List<Match>();
        public List<StandingCorrection> StandingCorrections { get; set; } = new List<StandingCorrection>();
    }
}
