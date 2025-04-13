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

     
        public async Task<IActionResult> Index()
        {
           
            var apiUrl = "https://localhost:7212/api/ProfessionGraph";

         
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
              
                return View("Error");
            }

         
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull  
            };

            var professionGraph = JsonSerializer.Deserialize<ProfessionGraphDTO>(jsonResponse, options);

           

          
            
            return View(professionGraph);
        }
    }
}
