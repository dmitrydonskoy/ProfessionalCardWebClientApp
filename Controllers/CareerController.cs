using Microsoft.AspNetCore.Mvc;
using ProfessionalCardWebClientApp.DTO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ProfessionalCardWebClientApp.Controllers
{
    public class CareerController : Controller
    {
        private readonly HttpClient _httpClient;

        public CareerController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index(int? targetProfessionId, string selectedCurrentProfessionName, string selectedCurrentProfessionLevel)
        {
            var professionsResponse = await _httpClient.GetAsync("https://localhost:7212/api/profession");
            if (!professionsResponse.IsSuccessStatusCode)
                return View("Error");

            var professionsJson = await professionsResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var allProfessions = JsonSerializer.Deserialize<List<ProfessionDTO>>(professionsJson, options);
            var availableProfessions = allProfessions.Where(p => string.IsNullOrEmpty(p.Level)).ToList();

            var viewModel = new CareerPathViewModel
            {
                AvailableProfessions = availableProfessions,
                SelectedProfessionId = targetProfessionId

            };
            viewModel.CareerPath = new CareerPathResponse
            {
                JuniorSkills = new List<SkillDTO>(),
                MiddleSkills = new List<SkillDTO>(),
                SeniorSkills = new List<SkillDTO>(),
                ManagerSkills = new List<SkillDTO>(),
                AlternativePaths = new List<AlternativeCareerPathDTO>()
            };

            if (targetProfessionId == null || string.IsNullOrWhiteSpace(selectedCurrentProfessionName) || string.IsNullOrWhiteSpace(selectedCurrentProfessionLevel))
            {
                return View(viewModel);
            }

            var selectedCurrentProfession = allProfessions
                .FirstOrDefault(p => p.Name == selectedCurrentProfessionName && p.Level == selectedCurrentProfessionLevel);

            if (selectedCurrentProfession == null)
                return View("Error");


            viewModel.SelectedCurrentProfessionId = selectedCurrentProfession.Id;
            viewModel.SelectedCurrentProfessionName = selectedCurrentProfession.Name;
            viewModel.SelectedCurrentProfessionLevel = selectedCurrentProfessionLevel;

            var apiUrl = $"https://localhost:7212/api/CareerPath/generate?" +
                         $"targetProfessionId={targetProfessionId}&" +
                         $"currentProfessionId={selectedCurrentProfession.Id}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            viewModel.CareerPath = JsonSerializer.Deserialize<CareerPathResponse>(jsonResponse, options);

            return View(viewModel);
        }
        public async Task<IActionResult> CompareWithSkills(int? targetProfessionId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            if (targetProfessionId == null)
                return RedirectToAction("Index");

            var professionsResponse = await _httpClient.GetAsync("https://localhost:7212/api/profession");
            if (!professionsResponse.IsSuccessStatusCode)
                return View("Error");

            var professionsJson = await professionsResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var allProfessions = JsonSerializer.Deserialize<List<ProfessionDTO>>(professionsJson, options);
            var availableProfessions = allProfessions.Where(p => string.IsNullOrEmpty(p.Level)).ToList();

            var viewModel = new CareerPathViewModel
            {
                AvailableProfessions = availableProfessions,
                SelectedProfessionId = targetProfessionId,
                IsPersonalizedComparison = true
            };

            var response = await _httpClient.GetAsync($"https://localhost:7212/api/CareerPath/compare?targetProfessionId={targetProfessionId}&userId={userId}");
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            viewModel.CareerPath = JsonSerializer.Deserialize<CareerPathResponse>(json, options);

            return View("Index", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddSkillToUser(int skillId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var response = await _httpClient.PostAsync($"https://localhost:7212/api/UserProfile/skills/{skillId}?userId={userId}", null);

            if (response.IsSuccessStatusCode)
                return Ok();

            return BadRequest("Не удалось добавить навык.");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToFavorites(int professionId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var response = await _httpClient.PostAsJsonAsync(
                $"https://localhost:7212/api/userprofile/{userId}/favorites", professionId
            );

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "Не удалось добавить профессию в избранное.";

            return RedirectToAction("Index", new { targetProfessionId = professionId });
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
                SelectedProfessionId = selectedProfession?.Id ?? professionId,
                SelectedCurrentProfessionName = selectedProfession?.Name,
                AvailableProfessions = allProfessions.Where(p => string.IsNullOrEmpty(p.Level)).ToList(),
                CareerPath = careerPath,
                IsPersonalizedComparison = true
            };

            return View("Index", viewModel);
        }

    }
}
