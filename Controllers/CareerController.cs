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

        public async Task<IActionResult> Index(int? targetProfessionId)
        {
            // Получаем список профессий
            var professionsResponse = await _httpClient.GetAsync("https://localhost:7212/api/profession");
            if (!professionsResponse.IsSuccessStatusCode)
            {
                return View("Error");
            }

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

            // Если профессию не выбрали — только отобразим список
            if (targetProfessionId == null)
                return View(viewModel);

            // Иначе подгружаем карьерный путь
            var apiUrl = $"https://localhost:7212/api/CareerPath/generate?targetProfessionId={targetProfessionId}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            viewModel.CareerPath = JsonSerializer.Deserialize<CareerPathDTO>(jsonResponse, options);

            return View(viewModel);
        }
    }
}
