using GoalZone.API.DTOs.StandingDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoalZone.API.Contexts;
using GoalZone.API.Enums;

namespace GoalZone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StandingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetStandings")]
        public async Task<List<StandingDto>> GetStandings(int seasonId)
        {
            var teamIds = await _context.Match
                .Where(x => x.SeasonId == seasonId)
                .Select(x => x.HomeTeamId)
                .Union(_context.Match
                    .Where(x => x.SeasonId == seasonId)
                    .Select(x => x.AwayTeamId))
                .ToListAsync();

            var teams = await _context.Team
                .Where(x => teamIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ToListAsync();

            var finishedMatches = await _context.Match
                .Where(x => x.SeasonId == seasonId && x.Status == MatchStatus.Finished)
                .OrderBy(x => x.MatchDate)
                .ToListAsync();

            var standings = new List<StandingDto>();

            foreach (var team in teams)
            {
                var teamMatches = finishedMatches
                    .Where(x => x.HomeTeamId == team.Id || x.AwayTeamId == team.Id)
                    .OrderBy(x => x.MatchDate)
                    .ToList();

                int played = teamMatches.Count;
                int won = 0;
                int drawn = 0;
                int lost = 0;
                int goalsFor = 0;
                int goalsAgainst = 0;
                int points = 0;

                foreach (var match in teamMatches)
                {
                    bool isHome = match.HomeTeamId == team.Id;

                    int teamGoals = isHome ? match.HomeFullTimeScore : match.AwayFullTimeScore;
                    int opponentGoals = isHome ? match.AwayFullTimeScore : match.HomeFullTimeScore;

                    goalsFor += teamGoals;
                    goalsAgainst += opponentGoals;

                    if (teamGoals > opponentGoals)
                    {
                        won++;
                        points += 3;
                    }
                    else if (teamGoals == opponentGoals)
                    {
                        drawn++;
                        points += 1;
                    }
                    else
                    {
                        lost++;
                    }
                }

                var form = teamMatches
                    .OrderByDescending(x => x.MatchDate)
                    .Take(5)
                    .Select(match =>
                    {
                        bool isHome = match.HomeTeamId == team.Id;

                        int teamGoals = isHome ? match.HomeFullTimeScore : match.AwayFullTimeScore;
                        int opponentGoals = isHome ? match.AwayFullTimeScore : match.HomeFullTimeScore;

                        if (teamGoals > opponentGoals) return "W";
                        if (teamGoals == opponentGoals) return "D";
                        return "L";
                    })
                    .Reverse()
                    .ToList();

                standings.Add(new StandingDto
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    ShortName = team.ShortName,
                    TeamLogoUrl = team.LogoUrl,
                    Played = played,
                    Won = won,
                    Drawn = drawn,
                    Lost = lost,
                    GoalsFor = goalsFor,
                    GoalsAgainst = goalsAgainst,
                    GoalDifference = goalsFor - goalsAgainst,
                    Points = points,
                    Form = form
                });
            }

            var orderedStandings = standings
                .OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.GoalDifference)
                .ThenByDescending(x => x.GoalsFor)
                .ThenBy(x => x.TeamName)
                .ToList();

            for (int i = 0; i < orderedStandings.Count; i++)
            {
                orderedStandings[i].Position = i + 1;
            }

            return orderedStandings;
        }

        [HttpGet("GetStandingsPage")]
        public async Task<StandingsPageDto?> GetStandingsPage(int seasonId)
        {
            var season = await _context.Season
                .FirstOrDefaultAsync(x => x.Id == seasonId);

            if (season == null)
                return null;

            var finishedMatches = await _context.Match
                .Where(x => x.SeasonId == seasonId && x.Status == MatchStatus.Finished)
                .Include(x => x.HomeTeam)
                .Include(x => x.AwayTeam)
                .OrderBy(x => x.MatchDate)
                .ToListAsync();

            if (!finishedMatches.Any())
                return new StandingsPageDto
                {
                    SeasonName = season.Name,
                    Subtitle = $"{season.Name} Sezonu",
                    Summary = new StandingSummaryDto
                    {
                        LeaderTeamName = "-",
                        LeaderPoints = 0,
                        MostGoalsTeamName = "-",
                        MostGoals = 0,
                        BestDefenseTeamName = "-",
                        LeastConceded = 0,
                        MatchOfTheWeekText = "-"
                    },
                    Standings = new List<StandingDto>()
                };

            var teamIds = finishedMatches
                .Select(x => x.HomeTeamId)
                .Union(finishedMatches.Select(x => x.AwayTeamId))
                .Distinct()
                .ToList();

            var teams = await _context.Team
                .Where(x => teamIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ToListAsync();

            var standings = new List<StandingDto>();

            foreach (var team in teams)
            {
                var teamMatches = finishedMatches
                    .Where(x => x.HomeTeamId == team.Id || x.AwayTeamId == team.Id)
                    .OrderBy(x => x.MatchDate)
                    .ToList();

                int played = teamMatches.Count;
                int won = 0;
                int drawn = 0;
                int lost = 0;
                int goalsFor = 0;
                int goalsAgainst = 0;
                int points = 0;

                foreach (var match in teamMatches)
                {
                    bool isHome = match.HomeTeamId == team.Id;

                    int teamGoals = isHome ? match.HomeFullTimeScore : match.AwayFullTimeScore;
                    int opponentGoals = isHome ? match.AwayFullTimeScore : match.HomeFullTimeScore;

                    goalsFor += teamGoals;
                    goalsAgainst += opponentGoals;

                    if (teamGoals > opponentGoals)
                    {
                        won++;
                        points += 3;
                    }
                    else if (teamGoals == opponentGoals)
                    {
                        drawn++;
                        points += 1;
                    }
                    else
                    {
                        lost++;
                    }
                }

                var form = teamMatches
                    .OrderByDescending(x => x.MatchDate)
                    .Take(5)
                    .Select(match =>
                    {
                        bool isHome = match.HomeTeamId == team.Id;

                        int teamGoals = isHome ? match.HomeFullTimeScore : match.AwayFullTimeScore;
                        int opponentGoals = isHome ? match.AwayFullTimeScore : match.HomeFullTimeScore;

                        if (teamGoals > opponentGoals) return "W";
                        if (teamGoals == opponentGoals) return "D";
                        return "L";
                    })
                    .Reverse()
                    .ToList();

                standings.Add(new StandingDto
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    ShortName = team.ShortName,
                    TeamLogoUrl = team.LogoUrl,
                    Played = played,
                    Won = won,
                    Drawn = drawn,
                    Lost = lost,
                    GoalsFor = goalsFor,
                    GoalsAgainst = goalsAgainst,
                    GoalDifference = goalsFor - goalsAgainst,
                    Points = points,
                    Form = form
                });
            }

            var orderedStandings = standings
                .OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.GoalDifference)
                .ThenByDescending(x => x.GoalsFor)
                .ThenBy(x => x.TeamName)
                .ToList();

            for (int i = 0; i < orderedStandings.Count; i++)
            {
                orderedStandings[i].Position = i + 1;
            }

            var leader = orderedStandings.First();

            var mostGoalsTeam = orderedStandings
                .OrderByDescending(x => x.GoalsFor)
                .ThenBy(x => x.TeamName)
                .First();

            var bestDefenseTeam = orderedStandings
                .OrderBy(x => x.GoalsAgainst)
                .ThenBy(x => x.TeamName)
                .First();

            var matchOfTheWeek = finishedMatches
                .OrderByDescending(x => x.HomeFullTimeScore + x.AwayFullTimeScore)
                .ThenByDescending(x => x.MatchDate)
                .First();

            var dto = new StandingsPageDto
            {
                SeasonName = season.Name,
                Subtitle = $"{season.Name} Sezonu",
                Summary = new StandingSummaryDto
                {
                    LeaderTeamName = leader.TeamName,
                    LeaderPoints = leader.Points,
                    MostGoalsTeamName = mostGoalsTeam.TeamName,
                    MostGoals = mostGoalsTeam.GoalsFor,
                    BestDefenseTeamName = bestDefenseTeam.TeamName,
                    LeastConceded = bestDefenseTeam.GoalsAgainst,
                    MatchOfTheWeekText = $"{matchOfTheWeek.HomeTeam.Name} {matchOfTheWeek.HomeFullTimeScore}-{matchOfTheWeek.AwayFullTimeScore} {matchOfTheWeek.AwayTeam.Name}"
                },
                Standings = orderedStandings
            };

            return dto;
        }
    }
}
