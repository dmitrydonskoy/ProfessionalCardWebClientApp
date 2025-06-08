using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProfessionalCardWebClientApp.DTO;
using System.Text;
using System.Text.Json;
namespace ProfessionalCardWebClientApp.Controllers
{
    
        public class AuthController : Controller
        {
            private readonly HttpClient _httpClient;

            public AuthController(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            // Страница регистрации
            public IActionResult Register()
            {
                return View();
            }
        [BindProperty]
        public LoginModel Model { get; set; }
        // Обработка регистрации
        [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Register(RegistrationModel model)
            {
                if (ModelState.IsValid)
                {
                    var jsonContent = JsonSerializer.Serialize(model);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // Отправка запроса на бэкенд для регистрации
                    var response = await _httpClient.PostAsync("https://localhost:7212/api/auth/register", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Ошибка при регистрации.");
                    }
                }

                return View(model);
            }

        public IActionResult Login()
        {
            var model = new LoginModel();
            ViewData["Title"] = "Авторизация";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://localhost:7212/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var loginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(json, options);

                    // Сохраняем userId и роль в сессию
                    HttpContext.Session.SetInt32("UserId", loginResponse.UserId);
                    HttpContext.Session.SetString("UserRole", loginResponse.Role);

                    // Проверяем роль
                    if (loginResponse.Role == "Admin")
                    {
                        // Перенаправляем администратора на админ-панель
                        return RedirectToAction("Index", "UserProfile");
                    }

                    // Иначе обычного пользователя на профиль
                    return RedirectToAction("Index", "UserProfile");
                }

                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
            }

            return View(model);
        }
        // Выход из системы
        public async Task<IActionResult> Logout()
            {
                await _httpClient.PostAsync("https://localhost:7212/api/auth/logout", null);
                return RedirectToAction("Index", "Home");
            }
        }
    }

