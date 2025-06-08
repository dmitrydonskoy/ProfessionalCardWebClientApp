using Microsoft.AspNetCore.Mvc;
using ProfessionalCardWebClientApp.DTO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ProfessionalCardWebClientApp.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserProfileController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var response = await _httpClient.GetAsync($"https://localhost:7212/api/UserProfile/{userId}");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var profile = JsonSerializer.Deserialize<UserProfileDTO>(json, options);

            // ✅ Фильтрация доступных профессий (только те, у которых Level != null)
            if (profile != null)
            {
                profile.AvailableProfessions = profile.AvailableProfessions
                    .Where(p => !string.IsNullOrWhiteSpace(p.Level))
                    .ToList();
            }

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int professionId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            await _httpClient.DeleteAsync($"https://localhost:7212/api/UserProfile/{userId}/favorites/{professionId}");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddCurrentProfession(int professionId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var response = await _httpClient.PostAsync(
                $"https://localhost:7212/api/UserProfile/professions/{professionId}?userId={userId}", null);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCurrentProfession(int professionId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            await _httpClient.DeleteAsync(
                $"https://localhost:7212/api/UserProfile/professions/{professionId}?userId={userId}");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddSkill(int skillId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            await _httpClient.PostAsync(
                $"https://localhost:7212/api/UserProfile/skills/{skillId}?userId={userId}", null);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSkill(int skillId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            await _httpClient.DeleteAsync(
                $"https://localhost:7212/api/UserProfile/skills/{skillId}?userId={userId}");

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> GenerateFromFavorite(int professionId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var apiUrl = $"https://localhost:7212/api/CareerPath/compare?targetProfessionId={professionId}&userId={userId}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var careerPath = JsonSerializer.Deserialize<CareerPathResponse>(json, options);

            var professionsResponse = await _httpClient.GetAsync("https://localhost:7212/api/profession");
            var allProfessionsJson = await professionsResponse.Content.ReadAsStringAsync();
            var allProfessions = JsonSerializer.Deserialize<List<ProfessionDTO>>(allProfessionsJson, options);

            var selectedProfession = allProfessions.FirstOrDefault(p => p.Id == professionId);

            var viewModel = new CareerPathViewModel
            {
                SelectedProfessionId = selectedProfession.Id,
                SelectedCurrentProfessionName = selectedProfession.Name,
                CareerPath = careerPath,
                AvailableProfessions = allProfessions.Where(p => string.IsNullOrEmpty(p.Level)).ToList()
            };

            return View("Index", viewModel);
        }

    }
}
