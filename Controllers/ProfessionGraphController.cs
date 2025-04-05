using Microsoft.AspNetCore.Mvc;
using ProfessionalCardWebClientApp.DTO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProfessionalCardWebClientApp.Controllers
{
    public class ProfessionGraphController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProfessionGraphController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Эндпоинт для получения карты профессий
        public async Task<IActionResult> Index()
        {
            // URL API для получения карты профессий
            var apiUrl = "https://localhost:7212/api/ProfessionGraph";

            // Отправляем GET-запрос к API
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                // Если запрос не удался, можно обработать ошибку или вернуть представление с ошибкой
                return View("Error");
            }

            // Читаем содержимое ответа
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,  // игнорировать регистр букв
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull  // игнорировать null значения
            };

            var professionGraph = JsonSerializer.Deserialize<ProfessionGraphDTO>(jsonResponse, options);

            // Десериализуем JSON-ответ в объект DTO
        

            // Передаем данные в представление
            
            return View(professionGraph);
        }
    }
}
