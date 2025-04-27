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
            var userId = HttpContext.Session.GetInt32("UserId"); // или другой способ

            if (userId == null)
            {
                // Пользователь не авторизован — редирект на логин
                return RedirectToAction("Login", "Auth");
            }

            // Получение профиля
            var response = await _httpClient.GetAsync($"https://localhost:7212/api/UserProfile/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var profile = JsonSerializer.Deserialize<UserProfileDTO>(json, options);

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int userId, int professionId)
        {
            var response = await _httpClient.PostAsJsonAsync($"https://localhost:7212/api/userprofile/{userId}/favorites", professionId);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Не удалось добавить профессию в избранное.";
            }

            return RedirectToAction("Index", new { userId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int userId, int professionId)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7212/api/userprofile/{userId}/favorites/{professionId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Не удалось удалить профессию из избранного.";
            }

            return RedirectToAction("Index", new { userId });
        }
    }
}
