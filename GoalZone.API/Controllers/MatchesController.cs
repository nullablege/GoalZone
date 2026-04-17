using System.ComponentModel.DataAnnotations;
using GoalZone.API.DTOs.AdminDTOs;
using GoalZone.API.DTOs.MatchDTOs;
using GoalZone.API.DTOs.PageDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using GoalZone.API.Contexts;
using GoalZone.API.Entities;

namespace GoalZone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MatchesController(AppDbContext context)
        {
            _context = context; 
        }

        [HttpGet("GetMatchCreatePage")]
        public async Task<MatchCreatePageDto> GetMatchCreatePage()
        {
            return new MatchCreatePageDto
            {
                Form = new AddMatchDto
                {
                    MatchDate = DateTime.Now.AddDays(1),
                    Status = GoalZone.API.Enums.MatchStatus.NotStarted
                },
                Seasons = await _context.Season
                    .OrderByDescending(x => x.Name)
                    .Select(x => new AdminLookupItemDto
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                    .ToListAsync(),
                Teams = await _context.Team
                    .OrderBy(x => x.Name)
                    .Select(x => new AdminLookupItemDto
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                    .ToListAsync(),
                Statuses = Enum.GetValues<GoalZone.API.Enums.MatchStatus>()
                    .Select(x => new AdminLookupItemDto
                    {
                        Id = (int)x,
                        Name = x switch
                        {
                            GoalZone.API.Enums.MatchStatus.NotStarted => "Başlamadı",
                            GoalZone.API.Enums.MatchStatus.Live => "Canlı",
                            GoalZone.API.Enums.MatchStatus.Finished => "Bitti",
                            _ => x.ToString()
                        }
                    })
                    .ToList()
            };
        }

        [HttpPost("AddMatch")]
        public async Task<IActionResult> AddMatch(AddMatchDto dto)
        {
            if (dto.HomeTeamId == dto.AwayTeamId)
            {
                ModelState.AddModelError(nameof(dto.AwayTeamId), "Ev sahibi ve deplasman takımı aynı olamaz.");
            }

            if (!await _context.Season.AnyAsync(x => x.Id == dto.SeasonId))
            {
                ModelState.AddModelError(nameof(dto.SeasonId), "Seçilen sezon bulunamadı.");
            }

            if (!await _context.Team.AnyAsync(x => x.Id == dto.HomeTeamId))
            {
                ModelState.AddModelError(nameof(dto.HomeTeamId), "Ev sahibi takımı bulunamadı.");
            }

            if (!await _context.Team.AnyAsync(x => x.Id == dto.AwayTeamId))
            {
                ModelState.AddModelError(nameof(dto.AwayTeamId), "Deplasman takımı bulunamadı.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var nextMatchId = await _context.Match
                .Select(x => (int?)x.Id)
                .MaxAsync() ?? 0;

            var match = new Match
            {
                Id = nextMatchId + 1,
                SeasonId = dto.SeasonId,
                WeekNumber = dto.WeekNumber,
                MatchDate = dto.MatchDate,
                HomeTeamId = dto.HomeTeamId,
                AwayTeamId = dto.AwayTeamId,
                StadiumName = dto.StadiumName.Trim(),
                Status = dto.Status,
                IsFeaturedMatch = dto.IsFeaturedMatch,
                HomeHalfTimeScore = dto.HomeHalfTimeScore,
                AwayHalfTimeScore = dto.AwayHalfTimeScore,
                HomeFullTimeScore = dto.HomeFullTimeScore,
                AwayFullTimeScore = dto.AwayFullTimeScore
            };

            _context.Match.Add(match);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMatchDetail), new { id = match.Id }, new { match.Id });
        }

        [HttpGet("GetEventCreatePage")]
        public async Task<EventCreatePageDto> GetEventCreatePage()
        {
            return new EventCreatePageDto
            {
                Matches = await _context.Match
                    .Include(x => x.HomeTeam)
                    .Include(x => x.AwayTeam)
                    .OrderByDescending(x => x.MatchDate)
                    .Select(x => new AdminMatchOptionDto
                    {
                        Id = x.Id,
                        Name = x.HomeTeam.Name + " vs " + x.AwayTeam.Name + " (" + x.MatchDate.ToString("dd.MM.yyyy") + ")",
                        HomeTeamId = x.HomeTeamId,
                        AwayTeamId = x.AwayTeamId
                    })
                    .ToListAsync(),
                Teams = await _context.Team
                    .OrderBy(x => x.Name)
                    .Select(x => new AdminLookupItemDto
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                    .ToListAsync(),
                Players = await _context.Player
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .Select(x => new AdminPlayerOptionDto
                    {
                        Id = x.Id,
                        Name = x.FirstName + " " + x.LastName,
                        TeamId = x.TeamId
                    })
                    .ToListAsync(),
                EventTypes = Enum.GetValues<GoalZone.API.Enums.MatchEventType>()
                    .Select(x => new AdminLookupItemDto
                    {
                        Id = (int)x,
                        Name = x switch
                        {
                            GoalZone.API.Enums.MatchEventType.Goal => "Goal",
                            GoalZone.API.Enums.MatchEventType.YellowCard => "Yellow Card",
                            GoalZone.API.Enums.MatchEventType.RedCard => "Red Card",
                            _ => x.ToString()
                        }
                    })
                    .ToList()
            };
        }

        [HttpPost("AddEvent")]
        public async Task<IActionResult> AddEvent(AddEventDto dto)
        {
            var match = await _context.Match
                .FirstOrDefaultAsync(x => x.Id == dto.MatchId);

            if (match == null)
            {
                ModelState.AddModelError("", "Seçilen maç bulunamadı.");
            }

            var team = await _context.Team
                .FirstOrDefaultAsync(x => x.Id == dto.TeamId);

            if (team == null)
            {
                ModelState.AddModelError("", "Seçilen takım bulunamadı.");
            }
            else if (match != null && team.Id != match.HomeTeamId && team.Id != match.AwayTeamId)
            {
                ModelState.AddModelError("", "Seçilen takım maçın taraflarından biri olmalıdır.");
            }

            var player = await _context.Player
                .FirstOrDefaultAsync(x => x.Id == dto.PlayerId);

            if (player == null)
            {
                ModelState.AddModelError("", "Seçilen oyuncu bulunamadı.");
            }
            else if (player.TeamId != dto.TeamId)
            {
                ModelState.AddModelError("", "Oyuncu seçilen takıma ait olmalıdır.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var nextEventId = await _context.MatchEvent
                .Select(x => (int?)x.Id)
                .MaxAsync() ?? 0;

            var matchEvent = new MatchEvent
            {
                Id = nextEventId + 1,
                MatchId = dto.MatchId,
                TeamId = dto.TeamId,
                PlayerId = dto.PlayerId,
                EventType = dto.EventType,
                Minute = dto.Minute,
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim()
            };

            _context.MatchEvent.Add(matchEvent);

            if (dto.EventType == GoalZone.API.Enums.MatchEventType.YellowCard || dto.EventType == GoalZone.API.Enums.MatchEventType.RedCard)
            {
                var statistic = await _context.MatchStatistic
                    .FirstOrDefaultAsync(x => x.MatchId == dto.MatchId);

                if (statistic == null)
                {
                    statistic = new MatchStatistic
                    {
                        MatchId = dto.MatchId
                    };

                    _context.MatchStatistic.Add(statistic);
                }

                if (dto.TeamId == match!.HomeTeamId)
                {
                    if (dto.EventType == GoalZone.API.Enums.MatchEventType.YellowCard)
                    {
                        statistic.HomeYellowCards += 1;
                    }

                    if (dto.EventType == GoalZone.API.Enums.MatchEventType.RedCard)
                    {
                        statistic.HomeRedCards += 1;
                    }
                }
                else
                {
                    if (dto.EventType == GoalZone.API.Enums.MatchEventType.YellowCard)
                    {
                        statistic.AwayYellowCards += 1;
                    }

                    if (dto.EventType == GoalZone.API.Enums.MatchEventType.RedCard)
                    {
                        statistic.AwayRedCards += 1;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMatchDetail), new { id = dto.MatchId }, new { matchEvent.Id });
        }

        [HttpPost("AddSubstitution")]
        public async Task<IActionResult> AddSubstitution(AddSubstitutionDto dto)
        {
            var match = await _context.Match
                .FirstOrDefaultAsync(x => x.Id == dto.MatchId);

            if (match == null)
            {
                ModelState.AddModelError("", "Seçilen maç bulunamadı.");
            }

            var team = await _context.Team
                .FirstOrDefaultAsync(x => x.Id == dto.TeamId);

            if (team == null)
            {
                ModelState.AddModelError("", "Seçilen takım bulunamadı.");
            }
            else if (match != null && team.Id != match.HomeTeamId && team.Id != match.AwayTeamId)
            {
                ModelState.AddModelError("", "Seçilen takım maçın taraflarından biri olmalıdır.");
            }

            var playerOut = await _context.Player
                .FirstOrDefaultAsync(x => x.Id == dto.PlayerOutId);

            if (playerOut == null)
            {
                ModelState.AddModelError("", "Oyundan çıkacak oyuncu bulunamadı.");
            }
            else if (playerOut.TeamId != dto.TeamId)
            {
                ModelState.AddModelError("", "Oyundan çıkacak oyuncu seçilen takıma ait olmalıdır.");
            }

            var playerIn = await _context.Player
                .FirstOrDefaultAsync(x => x.Id == dto.PlayerInId);

            if (playerIn == null)
            {
                ModelState.AddModelError("", "Oyuna girecek oyuncu bulunamadı.");
            }
            else if (playerIn.TeamId != dto.TeamId)
            {
                ModelState.AddModelError("", "Oyuna girecek oyuncu seçilen takıma ait olmalıdır.");
            }

            if (dto.PlayerOutId == dto.PlayerInId)
            {
                ModelState.AddModelError("", "Oyuna giren ve çıkan oyuncu aynı olamaz.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var nextSubstitutionId = await _context.MatchSubstitution
                .Select(x => (int?)x.Id)
                .MaxAsync() ?? 0;

            var substitution = new MatchSubstitution
            {
                Id = nextSubstitutionId + 1,
                MatchId = dto.MatchId,
                TeamId = dto.TeamId,
                PlayerOutId = dto.PlayerOutId,
                PlayerInId = dto.PlayerInId,
                Minute = dto.Minute
            };

            _context.MatchSubstitution.Add(substitution);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMatchDetail), new { id = dto.MatchId }, new { substitution.Id });
        }

        [HttpGet("GetStatisticCreatePage")]
        public async Task<StatisticCreatePageDto> GetStatisticCreatePage(int? matchId)
        {
            var page = new StatisticCreatePageDto
            {
                Matches = await _context.Match
                    .Include(x => x.HomeTeam)
                    .Include(x => x.AwayTeam)
                    .OrderByDescending(x => x.MatchDate)
                    .Select(x => new AdminMatchOptionDto
                    {
                        Id = x.Id,
                        Name = x.HomeTeam.Name + " vs " + x.AwayTeam.Name + " (" + x.MatchDate.ToString("dd.MM.yyyy") + ")",
                        HomeTeamId = x.HomeTeamId,
                        AwayTeamId = x.AwayTeamId
                    })
                    .ToListAsync()
            };

            if (matchId.HasValue)
            {
                page.Form.MatchId = matchId.Value;

                var statistic = await _context.MatchStatistic
                    .FirstOrDefaultAsync(x => x.MatchId == matchId.Value);

                if (statistic != null)
                {
                    page.Form.HomePossession = statistic.HomePossession;
                    page.Form.AwayPossession = statistic.AwayPossession;
                    page.Form.HomeShots = statistic.HomeShots;
                    page.Form.AwayShots = statistic.AwayShots;
                    page.Form.HomeShotsOnTarget = statistic.HomeShotsOnTarget;
                    page.Form.AwayShotsOnTarget = statistic.AwayShotsOnTarget;
                    page.Form.HomeCorners = statistic.HomeCorners;
                    page.Form.AwayCorners = statistic.AwayCorners;
                    page.Form.HomeFouls = statistic.HomeFouls;
                    page.Form.AwayFouls = statistic.AwayFouls;
                    page.Form.HomeYellowCards = statistic.HomeYellowCards;
                    page.Form.AwayYellowCards = statistic.AwayYellowCards;
                    page.Form.HomeRedCards = statistic.HomeRedCards;
                    page.Form.AwayRedCards = statistic.AwayRedCards;
                }
            }

            return page;
        }

        [HttpPost("AddStatistic")]
        public async Task<IActionResult> AddStatistic(AddStatisticsDto dto)
        {
            var match = await _context.Match
                .FirstOrDefaultAsync(x => x.Id == dto.MatchId);

            if (match == null)
            {
                ModelState.AddModelError("", "Seçilen maç bulunamadı.");
            }

            if (dto.HomePossession + dto.AwayPossession != 100)
            {
                ModelState.AddModelError("", "Topla oynama yüzdeleri toplamı 100 olmalıdır.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var statistic = await _context.MatchStatistic
                .FirstOrDefaultAsync(x => x.MatchId == dto.MatchId);

            if (statistic == null)
            {
                statistic = new MatchStatistic
                {
                    MatchId = dto.MatchId
                };

                _context.MatchStatistic.Add(statistic);
            }

            statistic.HomePossession = dto.HomePossession;
            statistic.AwayPossession = dto.AwayPossession;
            statistic.HomeShots = dto.HomeShots;
            statistic.AwayShots = dto.AwayShots;
            statistic.HomeShotsOnTarget = dto.HomeShotsOnTarget;
            statistic.AwayShotsOnTarget = dto.AwayShotsOnTarget;
            statistic.HomeCorners = dto.HomeCorners;
            statistic.AwayCorners = dto.AwayCorners;
            statistic.HomeFouls = dto.HomeFouls;
            statistic.AwayFouls = dto.AwayFouls;
            statistic.HomeYellowCards = dto.HomeYellowCards;
            statistic.AwayYellowCards = dto.AwayYellowCards;
            statistic.HomeRedCards = dto.HomeRedCards;
            statistic.AwayRedCards = dto.AwayRedCards;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMatchDetail), new { id = dto.MatchId }, new { statistic.Id });
        }

        [HttpGet("GetFeaturedMatch")]
        public async Task<ResultFeatureMatchDto?> GetFeaturedMatch(int weekNumber)
        {
            var value = await _context.Match
                .Where(x => x.WeekNumber == weekNumber && x.IsFeaturedMatch)
                .Select(x => new ResultFeatureMatchDto
                {
                    Id = x.Id,
                    HomeTeamName = x.HomeTeam.Name,
                    AwayTeamName = x.AwayTeam.Name,
                    HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                    AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                    HomeHalfTimeScore = x.HomeHalfTimeScore,
                    AwayHalfTimeScore = x.AwayHalfTimeScore,
                    HomeFullTimeScore = x.HomeFullTimeScore,
                    AwayFullTimeScore = x.AwayFullTimeScore,
                    MatchDate = x.MatchDate,
                    StadiumName = x.StadiumName
                })
                .FirstOrDefaultAsync();

            if (value == null)
            {
                value = await _context.Match
                    .Where(x => x.WeekNumber == weekNumber)
                    .OrderByDescending(x => x.HomeFullTimeScore + x.AwayFullTimeScore)
                    .Select(x => new ResultFeatureMatchDto
                    {
                        Id = x.Id,
                        HomeTeamName = x.HomeTeam.Name,
                        AwayTeamName = x.AwayTeam.Name,
                        HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                        AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                        HomeHalfTimeScore = x.HomeHalfTimeScore,
                        AwayHalfTimeScore = x.AwayHalfTimeScore,
                        HomeFullTimeScore = x.HomeFullTimeScore,
                        AwayFullTimeScore = x.AwayFullTimeScore,
                        MatchDate = x.MatchDate,
                        StadiumName = x.StadiumName
                    })
                    .FirstOrDefaultAsync();
            }

            return value;
        }

        [HttpGet("GetLiveMatches")]
        public async Task<List<ResultMatchDto>> GetLiveMatches(int weekNumber)
        {
            var values = await _context.Match.Where(x => x.WeekNumber == weekNumber &&  x.Status == GoalZone.API.Enums.MatchStatus.Live )
                .OrderBy(x => x.MatchDate)
                .Select(x => new ResultMatchDto
            {
                Id = x.Id,
                HomeTeamName = x.HomeTeam.Name,
                AwayTeamName = x.AwayTeam.Name,
                HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                HomeFullTimeScore = x.HomeFullTimeScore,
                AwayFullTimeScore = x.AwayFullTimeScore,
                MatchDate = x.MatchDate,
            }).ToListAsync();

           

            return values;
        }


        [HttpGet("GetFinishedMatches")]
        public async Task<List<ResultMatchDto>> GetFinishedMatches(int weekNumber)
        {
            var values = await _context.Match.Where(x => x.WeekNumber == weekNumber && x.Status == GoalZone.API.Enums.MatchStatus.Finished)
                .OrderBy(x => x.MatchDate)
                .Select(x => new ResultMatchDto
                {
                    Id = x.Id,
                    HomeTeamName = x.HomeTeam.Name,
                    AwayTeamName = x.AwayTeam.Name,
                    HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                    AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                    HomeFullTimeScore = x.HomeFullTimeScore,
                    AwayFullTimeScore = x.AwayFullTimeScore,
                    MatchDate = x.MatchDate,
                }).ToListAsync();



            return values;
        }


        [HttpGet("GetUpcomingMatches")]
        public async Task<List<ResultMatchDto>> GetUpcomingMatches(int weekNumber)
        {
            var values = await _context.Match.Where(x => x.WeekNumber == weekNumber && x.Status == GoalZone.API.Enums.MatchStatus.NotStarted)
                .OrderBy(x => x.MatchDate)
                .Select(x => new ResultMatchDto
                {
                    Id = x.Id,
                    HomeTeamName = x.HomeTeam.Name,
                    AwayTeamName = x.AwayTeam.Name,
                    HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                    AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                    HomeFullTimeScore = x.HomeFullTimeScore,
                    AwayFullTimeScore = x.AwayFullTimeScore,
                    MatchDate = x.MatchDate,
                }).ToListAsync();



            return values;
        }
        [HttpGet("GetFixtures")]
        public async Task<List<FixtureMatchDto>> GetFixtures(int weekNumber)
        {
            var values = await _context.Match
                .Where(x => x.WeekNumber == weekNumber && x.Status == GoalZone.API.Enums.MatchStatus.NotStarted)
                .OrderBy(x => x.MatchDate)
                .Select(x => new FixtureMatchDto
                {
                    Id = x.Id,
                    HomeTeamName = x.HomeTeam.Name,
                    AwayTeamName = x.AwayTeam.Name,
                    HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                    AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                    MatchDate = x.MatchDate,
                    StadiumName = x.StadiumName,
                    WeekNumber = x.WeekNumber,
                    IsFeaturedMatch = x.IsFeaturedMatch
                })
                .ToListAsync();

            return values;
        }

        [HttpGet("{id}")]
        public async Task<MatchDetailDto?> GetMatchDetail(int id)
        {
            var match = await _context.Match
                .Include(x => x.HomeTeam)
                .Include(x => x.AwayTeam)
                .Include(x => x.Season)
                .Include(x => x.Info)
                .Include(x => x.Statistic)
                .Include(x => x.Events).ThenInclude(x => x.Player)
                .Include(x => x.Substitutions).ThenInclude(x => x.PlayerIn)
                .Include(x => x.Substitutions).ThenInclude(x => x.PlayerOut)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (match == null) return null;

            var dto = new MatchDetailDto
            {
                Id = match.Id,
                HomeTeamName = match.HomeTeam.Name,
                AwayTeamName = match.AwayTeam.Name,
                HomeTeamLogoUrl = match.HomeTeam.LogoUrl,
                AwayTeamLogoUrl = match.AwayTeam.LogoUrl,
                HomeHalfTimeScore = match.HomeHalfTimeScore,
                AwayHalfTimeScore = match.AwayHalfTimeScore,
                HomeFullTimeScore = match.HomeFullTimeScore,
                AwayFullTimeScore = match.AwayFullTimeScore,
                Status = match.Status.ToString(),
                MatchDate = match.MatchDate,
                StadiumName = match.StadiumName,
                WeekNumber = match.WeekNumber,
                SeasonName = match.Season.Name,
                RefereeName = match.Info?.RefereeName,
                Attendance = match.Info?.Attendance,
                Statistics = match.Statistic == null ? null : new MatchStatisticDto
                {
                    HomePossession = match.Statistic.HomePossession,
                    AwayPossession = match.Statistic.AwayPossession,
                    HomeShots = match.Statistic.HomeShots,
                    AwayShots = match.Statistic.AwayShots,
                    HomeShotsOnTarget = match.Statistic.HomeShotsOnTarget,
                    AwayShotsOnTarget = match.Statistic.AwayShotsOnTarget,
                    HomeCorners = match.Statistic.HomeCorners,
                    AwayCorners = match.Statistic.AwayCorners,
                    HomeFouls = match.Statistic.HomeFouls,
                    AwayFouls = match.Statistic.AwayFouls,
                    HomeYellowCards = match.Statistic.HomeYellowCards,
                    AwayYellowCards = match.Statistic.AwayYellowCards,
                    HomeRedCards = match.Statistic.HomeRedCards,
                    AwayRedCards = match.Statistic.AwayRedCards
                },
                Timeline = match.Events
                    .OrderBy(x => x.Minute)
                    .Select(x => new MatchEventDto
                    {
                        Minute = x.Minute,
                        TeamSide = x.TeamId == match.HomeTeamId ? "Home" : "Away",
                        PlayerName = x.Player.FirstName + " " + x.Player.LastName,
                        EventType = x.EventType.ToString(),
                        Description = x.Description
                    }).ToList(),
                Goals = match.Events
                    .Where(x => x.EventType == GoalZone.API.Enums.MatchEventType.Goal)
                    .OrderBy(x => x.Minute)
                    .Select(x => new GoalDto
                    {
                        Minute = x.Minute,
                        TeamSide = x.TeamId == match.HomeTeamId ? "Home" : "Away",
                        PlayerName = x.Player.FirstName + " " + x.Player.LastName
                    }).ToList(),
                Cards = match.Events
                    .Where(x => x.EventType == GoalZone.API.Enums.MatchEventType.YellowCard
                             || x.EventType == GoalZone.API.Enums.MatchEventType.RedCard)
                    .OrderBy(x => x.Minute)
                    .Select(x => new CardDto
                    {
                        Minute = x.Minute,
                        TeamSide = x.TeamId == match.HomeTeamId ? "Home" : "Away",
                        PlayerName = x.Player.FirstName + " " + x.Player.LastName,
                        CardType = x.EventType.ToString()
                    }).ToList(),
                Substitutions = match.Substitutions
                    .OrderBy(x => x.Minute)
                    .Select(x => new SubstitutionDto
                    {
                        Minute = x.Minute,
                        TeamSide = x.TeamId == match.HomeTeamId ? "Home" : "Away",
                        PlayerInName = x.PlayerIn.FirstName + " " + x.PlayerIn.LastName,
                        PlayerOutName = x.PlayerOut.FirstName + " " + x.PlayerOut.LastName
                    }).ToList()
            };

            return dto;
        }

        [HttpGet("GetIndexPage")]
        public async Task<IndexPageDto?> GetIndexPage(int weekNumber)
        {
            var matches = await _context.Match
                .Where(x => x.WeekNumber == weekNumber)
                .Include(x => x.HomeTeam)
                .Include(x => x.AwayTeam)
                .OrderBy(x => x.MatchDate)
                .ToListAsync();

            if (!matches.Any())
                return null;

            var liveMatches = matches
                .Where(x => x.Status == GoalZone.API.Enums.MatchStatus.Live)
                .Select(x => new ResultMatchDto
                {
                    Id = x.Id,
                    HomeTeamName = x.HomeTeam.Name,
                    AwayTeamName = x.AwayTeam.Name,
                    HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                    AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                    HomeFullTimeScore = x.HomeFullTimeScore,
                    AwayFullTimeScore = x.AwayFullTimeScore,
                    MatchDate = x.MatchDate,
                    StadiumName = x.StadiumName
                })
                .ToList();

            var finishedMatches = matches
                .Where(x => x.Status == GoalZone.API.Enums.MatchStatus.Finished)
                .Select(x => new ResultMatchDto
                {
                    Id = x.Id,
                    HomeTeamName = x.HomeTeam.Name,
                    AwayTeamName = x.AwayTeam.Name,
                    HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                    AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                    HomeFullTimeScore = x.HomeFullTimeScore,
                    AwayFullTimeScore = x.AwayFullTimeScore,
                    MatchDate = x.MatchDate,
                    StadiumName = x.StadiumName
                })
                .ToList();

            var upcomingMatches = matches
                .Where(x => x.Status == GoalZone.API.Enums.MatchStatus.NotStarted)
                .Select(x => new ResultMatchDto
                {
                    Id = x.Id,
                    HomeTeamName = x.HomeTeam.Name,
                    AwayTeamName = x.AwayTeam.Name,
                    HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                    AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                    HomeFullTimeScore = x.HomeFullTimeScore,
                    AwayFullTimeScore = x.AwayFullTimeScore,
                    MatchDate = x.MatchDate,
                    StadiumName = x.StadiumName
                })
                .ToList();

            var featuredMatch = matches
                .Where(x => x.IsFeaturedMatch)
                .Select(x => new ResultFeatureMatchDto
                {
                    Id = x.Id,
                    HomeTeamName = x.HomeTeam.Name,
                    AwayTeamName = x.AwayTeam.Name,
                    HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                    AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                    HomeFullTimeScore = x.HomeFullTimeScore,
                    AwayFullTimeScore = x.AwayFullTimeScore,
                    HomeHalfTimeScore = x.HomeHalfTimeScore,
                    AwayHalfTimeScore = x.AwayHalfTimeScore,
                    MatchDate = x.MatchDate,
                    StadiumName = x.StadiumName
                })
                .FirstOrDefault();

            if (featuredMatch == null)
            {
                featuredMatch = matches
                    .OrderByDescending(x => x.HomeFullTimeScore + x.AwayFullTimeScore)
                    .Select(x => new ResultFeatureMatchDto
                    {
                        Id = x.Id,
                        HomeTeamName = x.HomeTeam.Name,
                        AwayTeamName = x.AwayTeam.Name,
                        HomeTeamLogoUrl = x.HomeTeam.LogoUrl,
                        AwayTeamLogoUrl = x.AwayTeam.LogoUrl,
                        HomeFullTimeScore = x.HomeFullTimeScore,
                        AwayFullTimeScore = x.AwayFullTimeScore,
                        HomeHalfTimeScore = x.HomeHalfTimeScore,
                        AwayHalfTimeScore = x.AwayHalfTimeScore,
                        MatchDate = x.MatchDate,
                        StadiumName = x.StadiumName
                    })
                    .FirstOrDefault();
            }

            var minDate = matches.Min(x => x.MatchDate);
            var maxDate = matches.Max(x => x.MatchDate);

            var culture = new System.Globalization.CultureInfo("tr-TR");

            var dto = new IndexPageDto
            {
                WeekNumber = weekNumber,
                DateRangeText = $"{minDate:dd MMMM} - {maxDate:dd MMMM yyyy}",
                TotalMatchCount = matches.Count,
                LiveMatchCount = liveMatches.Count,
                FinishedMatchCount = finishedMatches.Count,
                UpcomingMatchCount = upcomingMatches.Count,
                FeaturedMatch = featuredMatch,
                LiveMatches = liveMatches,
                FinishedMatches = finishedMatches,
                UpcomingMatches = upcomingMatches
            };

            return dto;
        }

        [HttpGet("GetFixturesPage")]
        public async Task<FixturesPageDto?> GetFixturesPage(int weekNumber)
        {
            var matches = await _context.Match
                .Where(x => x.WeekNumber == weekNumber && x.Status == GoalZone.API.Enums.MatchStatus.NotStarted)
                .Include(x => x.HomeTeam)
                .Include(x => x.AwayTeam)
                .OrderBy(x => x.MatchDate)
                .ToListAsync();

            if (!matches.Any())
                return null;

            var startDate = matches.Min(x => x.MatchDate);
            var endDate = matches.Max(x => x.MatchDate);

            var fixturePage = new FixturesPageDto
            {
                WeekNumber = weekNumber,
                StartDate = startDate,
                EndDate = endDate,
                MatchCount = matches.Count,
                TopMatchCount = matches.Count(x => x.IsFeaturedMatch)
            };

            var dayGroups = matches
                .GroupBy(x => x.MatchDate.Date)
                .OrderBy(x => x.Key)
                .ToList();

            foreach (var dayGroup in dayGroups)
            {
                var fixtureDayGroupDto = new FixtureDayGroupDto
                {
                    DayName = dayGroup.Key.ToString("dddd", new System.Globalization.CultureInfo("tr-TR")),
                    Date = dayGroup.Key,
                    MatchCount = dayGroup.Count(),
                    Matches = new List<FixtureMatchDto>()
                };

                foreach (var match in dayGroup)
                {
                    var homeLast5Matches = await _context.Match
                        .Where(x =>
                            x.Status == GoalZone.API.Enums.MatchStatus.Finished &&
                            x.MatchDate < match.MatchDate &&
                            (x.HomeTeamId == match.HomeTeamId || x.AwayTeamId == match.HomeTeamId))
                        .OrderByDescending(x => x.MatchDate)
                        .Take(5)
                        .ToListAsync();

                    var awayLast5Matches = await _context.Match
                        .Where(x =>
                            x.Status == GoalZone.API.Enums.MatchStatus.Finished &&
                            x.MatchDate < match.MatchDate &&
                            (x.HomeTeamId == match.AwayTeamId || x.AwayTeamId == match.AwayTeamId))
                        .OrderByDescending(x => x.MatchDate)
                        .Take(5)
                        .ToListAsync();

                    var fixtureMatchDto = new FixtureMatchDto
                    {
                        Id = match.Id,
                        HomeTeamName = match.HomeTeam.Name,
                        AwayTeamName = match.AwayTeam.Name,
                        HomeTeamLogoUrl = match.HomeTeam.LogoUrl,
                        AwayTeamLogoUrl = match.AwayTeam.LogoUrl,
                        MatchDate = match.MatchDate,
                        StadiumName = match.StadiumName,
                        WeekNumber = match.WeekNumber,
                        IsFeaturedMatch = match.IsFeaturedMatch,
                        HomeLast5Matches = homeLast5Matches
                            .OrderBy(x => x.MatchDate)
                            .Select(x => GetMatchResult(x, match.HomeTeamId))
                            .ToList(),
                        AwayLast5Matches = awayLast5Matches
                            .OrderBy(x => x.MatchDate)
                            .Select(x => GetMatchResult(x, match.AwayTeamId))
                            .ToList()
                    };

                    fixtureDayGroupDto.Matches.Add(fixtureMatchDto);
                }

                fixturePage.DayGroups.Add(fixtureDayGroupDto);
            }

            return fixturePage;
        }

        private string GetMatchResult(Match match, int teamId)
        {
            bool isHomeTeam = match.HomeTeamId == teamId;

            int teamScore = isHomeTeam ? match.HomeFullTimeScore : match.AwayFullTimeScore;
            int opponentScore = isHomeTeam ? match.AwayFullTimeScore : match.HomeFullTimeScore;

            if (teamScore > opponentScore)
                return "W";

            if (teamScore == opponentScore)
                return "D";

            return "L";
        }

    }
}
