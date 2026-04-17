using GoalZone.WebUI.Models;
using GoalZone.API.DTOs.MatchDTOs;
using GoalZone.API.DTOs.PageDTOs;
using GoalZone.API.DTOs.StandingDTOs;
using Microsoft.AspNetCore.Mvc;

namespace GoalZone.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<IActionResult> Index(int weekNumber = 1)
        {
            var value = await _httpClient.GetFromJsonAsync<IndexPageDto?>($"https://localhost:7096/api/Matches/GetIndexPage?weekNumber={weekNumber}");

            if (value != null)
            {
                return View(new IndexViewModel
                {
                    WeekNumber = value.WeekNumber,
                    DateRangeText = value.DateRangeText,
                    TotalMatchCount = value.TotalMatchCount,
                    LiveMatchCount = value.LiveMatchCount,
                    FinishedMatchCount = value.FinishedMatchCount,
                    UpcomingMatchCount = value.UpcomingMatchCount,
                    FeaturedMatch = value.FeaturedMatch,
                    LiveMatches = value.LiveMatches,
                    FinishedMatches = value.FinishedMatches,
                    UpcomingMatches = value.UpcomingMatches
                });
            }

            return View(new IndexViewModel
            {
                WeekNumber = weekNumber,
                DateRangeText = "",
                FeaturedMatch = null,
                LiveMatches = new List<ResultMatchDto>(),
                FinishedMatches = new List<ResultMatchDto>(),
                UpcomingMatches = new List<ResultMatchDto>()
            });
        }
        public async Task<IActionResult> Fixtures(int weekNumber = 1)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7096/api/Matches/GetFixturesPage?weekNumber={weekNumber}");

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                var indexValue = await _httpClient.GetFromJsonAsync<IndexPageDto?>($"https://localhost:7096/api/Matches/GetIndexPage?weekNumber={weekNumber}");

                if (indexValue != null)
                {
                    var allMatches = indexValue.LiveMatches
                        .Concat(indexValue.FinishedMatches)
                        .Concat(indexValue.UpcomingMatches)
                        .OrderBy(x => x.MatchDate)
                        .ToList();

                    if (allMatches.Any())
                    {
                        var fallbackPage = new FixturesPageDto
                        {
                            WeekNumber = weekNumber,
                            StartDate = allMatches.Min(x => x.MatchDate),
                            EndDate = allMatches.Max(x => x.MatchDate),
                            MatchCount = allMatches.Count,
                            TopMatchCount = indexValue.FeaturedMatch != null ? 1 : 0
                        };

                        fallbackPage.DayGroups = allMatches
                            .GroupBy(x => x.MatchDate.Date)
                            .OrderBy(x => x.Key)
                            .Select(dayGroup => new FixtureDayGroupDto
                            {
                                DayName = dayGroup.Key.ToString("dddd", new System.Globalization.CultureInfo("tr-TR")),
                                Date = dayGroup.Key,
                                MatchCount = dayGroup.Count(),
                                Matches = dayGroup.Select(match => new FixtureMatchDto
                                {
                                    Id = match.Id,
                                    HomeTeamName = match.HomeTeamName,
                                    AwayTeamName = match.AwayTeamName,
                                    HomeTeamLogoUrl = match.HomeTeamLogoUrl,
                                    AwayTeamLogoUrl = match.AwayTeamLogoUrl,
                                    MatchDate = match.MatchDate,
                                    StadiumName = match.StadiumName ?? string.Empty,
                                    WeekNumber = weekNumber,
                                    IsFeaturedMatch = indexValue.FeaturedMatch?.Id == match.Id
                                }).ToList()
                            })
                            .ToList();

                        return View(fallbackPage);
                    }
                }

                return RedirectToAction("Index", new { weekNumber });
            }

            response.EnsureSuccessStatusCode();

            var value = await response.Content.ReadFromJsonAsync<FixturesPageDto?>();

            if (value != null)
            {
                return View(value);
            }

            return RedirectToAction("Index", new { weekNumber });
        }
        public async Task<IActionResult> Standings(int seasonId = 21646)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7096/api/Standings/GetStandingsPage?seasonId={seasonId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return RedirectToAction(nameof(Index));
            }

            response.EnsureSuccessStatusCode();

            var value = await response.Content.ReadFromJsonAsync<GoalZone.API.DTOs.StandingDTOs.StandingsPageDto?>();

            if (value != null)
            {
                return View(value);
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> MatchDetail(int id)
        {
            var value = await _httpClient.GetFromJsonAsync<MatchDetailDto?>($"https://localhost:7096/api/Matches/{id}");

            if (value != null)
            {
                return View(value);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
