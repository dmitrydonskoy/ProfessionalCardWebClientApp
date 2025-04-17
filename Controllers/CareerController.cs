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
            // Запрос профессий
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

            // Формируем модель для отображения
            var viewModel = new CareerPathViewModel
            {
                AvailableProfessions = availableProfessions,
                SelectedProfessionId = targetProfessionId
            };

            // Если профессия не выбрана или данные о текущей профессии не введены, возвращаем только форму с профессиями
            if (targetProfessionId == null || string.IsNullOrEmpty(selectedCurrentProfessionName) || string.IsNullOrEmpty(selectedCurrentProfessionLevel))
            {
                return View(viewModel);
            }

            // Находим текущую профессию по имени и уровню, если они заданы
            var selectedCurrentProfession = allProfessions
                .FirstOrDefault(p => p.Name == selectedCurrentProfessionName && p.Level == selectedCurrentProfessionLevel);

            // Если текущая профессия не найдена, возвращаем ошибку
            if (selectedCurrentProfession == null)
            {
                return View("Error"); // Или верните на страницу выбора профессий
            }

            // Обновляем модель с выбранной текущей профессией
            viewModel.SelectedCurrentProfessionId = selectedCurrentProfession.Id;
            viewModel.SelectedCurrentProfessionName = selectedCurrentProfession.Name;
            viewModel.SelectedCurrentProfessionLevel = selectedCurrentProfessionLevel;

            // Формируем запрос для генерации карьерного пути с учётом текущей профессии и уровня
            var apiUrl = $"https://localhost:7212/api/CareerPath/generate?" +
                         $"targetProfessionId={targetProfessionId}&" +
                         $"currentProfessionId={selectedCurrentProfession.Id}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var jsonResponse = await response.Content.ReadAsStringAsync();
            viewModel.CareerPath = JsonSerializer.Deserialize<CareerPathDTO>(jsonResponse, options);

            return View(viewModel);
        }

    }
}
