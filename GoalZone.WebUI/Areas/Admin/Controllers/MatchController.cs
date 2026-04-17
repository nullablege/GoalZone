using System.Net;
using GoalZone.API.DTOs.AdminDTOs;
using Microsoft.AspNetCore.Mvc;

namespace GoalZone.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MatchController : Controller
    {
        private readonly HttpClient _httpClient;

        public MatchController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var page = await GetMatchCreatePageAsync();
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MatchCreatePageDto model)
        {
            var form = model.Form;



            var response = await _httpClient.PostAsJsonAsync("https://localhost:7096/api/Matches/AddMatch", form);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Maç kaydı başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Create), new { created = true });
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                if (validationProblem?.Errors != null)
                {
                    foreach (var error in validationProblem.Errors)
                    {
                        foreach (var message in error.Value)
                        {
                            ModelState.AddModelError($"Form.{error.Key}", message);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Maç kaydı oluşturulurken beklenmeyen bir hata oluştu.");
            }

            var page = await GetMatchCreatePageAsync();
            page.Form = form;
            return View(page);
        }

        [HttpGet]
        public async Task<IActionResult> EventCreate()
        {
            var page = await GetEventCreatePageAsync();
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EventCreate(EventCreatePageDto model)
        {
            var form = model.Form;



            var response = await _httpClient.PostAsJsonAsync("https://localhost:7096/api/Matches/AddEvent", form);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Maç olayı başarıyla oluşturuldu.";
                return RedirectToAction("EventCreate");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                if (validationProblem?.Errors != null)
                {
                    foreach (var error in validationProblem.Errors)
                    {
                        foreach (var message in error.Value)
                        {
                            ModelState.AddModelError("", message);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Maç olayı oluşturulurken beklenmeyen bir hata oluştu.");
            }

            var page = await GetEventCreatePageAsync();
            page.Form = form;
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubstitutionCreate(EventCreatePageDto model)
        {
            var form = model.SubstitutionForm;

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7096/api/Matches/AddSubstitution", form);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Oyuncu değişikliği başarıyla oluşturuldu.";
                return RedirectToAction("EventCreate");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                if (validationProblem?.Errors != null)
                {
                    foreach (var error in validationProblem.Errors)
                    {
                        foreach (var message in error.Value)
                        {
                            ModelState.AddModelError("", message);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Oyuncu değişikliği oluşturulurken beklenmeyen bir hata oluştu.");
            }

            var page = await GetEventCreatePageAsync();
            page.Form = model.Form;
            page.SubstitutionForm = form;
            return View("EventCreate", page);
        }

        [HttpGet]
        public async Task<IActionResult> StatisticCreate(int? matchId)
        {
            var page = await GetStatisticCreatePageAsync(matchId);
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StatisticCreate(StatisticCreatePageDto model)
        {
            var form = model.Form;

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7096/api/Matches/AddStatistic", form);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Maç istatistiği başarıyla oluşturuldu.";
                return RedirectToAction("StatisticCreate", new { matchId = form.MatchId });
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                if (validationProblem?.Errors != null)
                {
                    foreach (var error in validationProblem.Errors)
                    {
                        foreach (var message in error.Value)
                        {
                            ModelState.AddModelError("", message);
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Maç istatistiği oluşturulurken beklenmeyen bir hata oluştu.");
            }

            var page = await GetStatisticCreatePageAsync(form.MatchId);
            page.Form = form;
            return View(page);
        }

        private async Task<MatchCreatePageDto> GetMatchCreatePageAsync()
        {
            var value = await _httpClient.GetFromJsonAsync<MatchCreatePageDto?>("https://localhost:7096/api/Matches/GetMatchCreatePage");
            return value ?? new MatchCreatePageDto();
        }

        private async Task<EventCreatePageDto> GetEventCreatePageAsync()
        {
            var value = await _httpClient.GetFromJsonAsync<EventCreatePageDto?>("https://localhost:7096/api/Matches/GetEventCreatePage");
            return value ?? new EventCreatePageDto();
        }

        private async Task<StatisticCreatePageDto> GetStatisticCreatePageAsync(int? matchId)
        {
            var url = "https://localhost:7096/api/Matches/GetStatisticCreatePage";

            if (matchId.HasValue)
            {
                url += $"?matchId={matchId.Value}";
            }

            var value = await _httpClient.GetFromJsonAsync<StatisticCreatePageDto?>(url);
            return value ?? new StatisticCreatePageDto();
        }
    }
}
