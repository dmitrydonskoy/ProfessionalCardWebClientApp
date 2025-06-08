using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.Json;
using System.Text;
using ProfessionalCardWebClientApp.DTO;

namespace ProfessionalCardWebClientApp.Controllers
{

   
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;

        public AdminController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public async Task<IActionResult> Index(
     string skillSearchTerm,
     string skillCategoryFilter,
     string professionSearchTerm,
     string professionCategoryFilter,
     string professionLevelFilter,
     string relationTypeFilter,
     string sourceProfessionFilterName,
string targetProfessionFilterName,
     int? sourceProfessionFilter,
     int? targetProfessionFilter,
     int page = 1,
     int pageSize = 10,
     string tab = "skills"
 )
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var responseSkills = await _httpClient.GetAsync("https://localhost:7212/api/Admin/Skills");
            var responseProfessions = await _httpClient.GetAsync("https://localhost:7212/api/Profession");
            var responseRelations = await _httpClient.GetAsync("https://localhost:7212/api/ProfessionRelation");

            if (!responseSkills.IsSuccessStatusCode || !responseProfessions.IsSuccessStatusCode || !responseRelations.IsSuccessStatusCode)
                return View("Error");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var skills = JsonSerializer.Deserialize<List<SkillDTO>>(await responseSkills.Content.ReadAsStringAsync(), options) ?? new();
            var professions = JsonSerializer.Deserialize<List<ProfessionDTO>>(await responseProfessions.Content.ReadAsStringAsync(), options) ?? new();
            var relations = JsonSerializer.Deserialize<List<ProfessionRelationDTO>>(await responseRelations.Content.ReadAsStringAsync(), options) ?? new();
            foreach (var relation in relations)
            {
                relation.SourceProfessionName = professions.FirstOrDefault(p => p.Id == relation.SourceProfessionId)?.Name ?? "Неизвестно";
                relation.TargetProfessionName = professions.FirstOrDefault(p => p.Id == relation.TargetProfessionId)?.Name ?? "Неизвестно";
            }

            // --- Фильтрация навыков ---
            if (!string.IsNullOrWhiteSpace(skillSearchTerm))
                skills = skills.Where(s => s.Name.Contains(skillSearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(skillCategoryFilter))
                skills = skills.Where(s => s.Category == skillCategoryFilter).ToList();

            // --- Фильтрация профессий ---
            if (!string.IsNullOrWhiteSpace(professionSearchTerm))
                professions = professions.Where(p => p.Name.Contains(professionSearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(professionCategoryFilter))
                professions = professions.Where(p => p.Category == professionCategoryFilter).ToList();
            if (!string.IsNullOrWhiteSpace(professionLevelFilter))
                professions = professions.Where(p => p.Level == professionLevelFilter).ToList();

            // Фильтрация связей по типу
            if (!string.IsNullOrWhiteSpace(relationTypeFilter))
                relations = relations.Where(r => r.RelationType.Contains(relationTypeFilter, StringComparison.OrdinalIgnoreCase)).ToList();

            // 🔍 Поиск профессий по названию
            if (!string.IsNullOrWhiteSpace(sourceProfessionFilterName))
            {
                var matched = professions
                    .Where(p => p.Name.Contains(sourceProfessionFilterName, StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.Id)
                    .ToList();
                relations = relations.Where(r => matched.Contains(r.SourceProfessionId)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(targetProfessionFilterName))
            {
                var matched = professions
                    .Where(p => p.Name.Contains(targetProfessionFilterName, StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.Id)
                    .ToList();
                relations = relations.Where(r => matched.Contains(r.TargetProfessionId)).ToList();
            }

            var viewModel = new AdminPanelViewModel
            {
                Skills = skills,
                Professions = professions,
                ProfessionRelations = relations,

                SkillSearchTerm = skillSearchTerm,
                SkillCategoryFilter = skillCategoryFilter,
                ProfessionSearchTerm = professionSearchTerm,
                ProfessionCategoryFilter = professionCategoryFilter,
                ProfessionLevelFilter = professionLevelFilter,
                RelationTypeFilter = relationTypeFilter,
                SourceProfessionFilterName = sourceProfessionFilterName,
                TargetProfessionFilterName = targetProfessionFilterName,

                CurrentPage = page,
                PageSize = pageSize
            };

            // 🟡 Добавим PaginationModel в ViewBag для каждой вкладки:
            ViewBag.SkillPagination = new PaginationModel
            {
                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double)viewModel.TotalSkills / pageSize),
                Tab = "skills",
                Filters = new Dictionary<string, string>
    {
        { "skillSearchTerm", skillSearchTerm ?? "" },
        { "skillCategoryFilter", skillCategoryFilter ?? "" }
    }
            };

            ViewBag.ProfessionPagination = new PaginationModel
            {
                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double)viewModel.TotalProfessions / pageSize),
                Tab = "professions",
                Filters = new Dictionary<string, string>
    {
        { "professionSearchTerm", professionSearchTerm ?? "" },
        { "professionCategoryFilter", professionCategoryFilter ?? "" },
        { "professionLevelFilter", professionLevelFilter ?? "" }
    }
            };

            ViewBag.RelationPagination = new PaginationModel
            {
                PageNumber = page,
                TotalPages = (int)Math.Ceiling((double)viewModel.TotalRelations / pageSize),
                Tab = "relations",
                Filters = new Dictionary<string, string>
    {
        { "relationTypeFilter", relationTypeFilter ?? "" },
      { "sourceProfessionFilterName", sourceProfessionFilterName ?? "" },
        { "targetProfessionFilterName", targetProfessionFilterName ?? "" }
    }
            };
            ViewBag.Professions = professions;
            ViewBag.ActiveTab = tab;
            return View(viewModel);
        }
   

        [HttpPost]
        public async Task<IActionResult> AddSkill(SkillDTO dto)
        {
            if (!IsAdmin()) return Unauthorized();

            if (string.IsNullOrWhiteSpace(dto.Description))
                dto.Description = "Автоматически добавлено";
                dto.HasSkill = true;

            // Нормализуем категорию
            dto.Category = dto.Category?.ToLower() == "it-skills" ? "IT-skills" : "experience";

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7212/api/Admin/AddSkill", dto);

            if (!response.IsSuccessStatusCode)
                return View("Error"); // Или лог, или TempData

            return RedirectToAction("Index", new { tab = "skills" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSkill(int id, SkillDTO dto)
        {
            if (!IsAdmin()) return Unauthorized();

            await _httpClient.PutAsJsonAsync($"https://localhost:7212/api/Admin/UpdateSkill/{id}", dto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            await _httpClient.DeleteAsync($"https://localhost:7212/api/Admin/DeleteSkill/{id}");
            return RedirectToAction("Index");
        }

        // ============== PROFESSIONS ================

        [HttpPost]
        public async Task<IActionResult> AddProfession(ProfessionDTO dto)
        {
            if (!IsAdmin()) return Unauthorized();

            await _httpClient.PostAsJsonAsync("https://localhost:7212/api/Admin/AddProfession", dto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfession(int id, ProfessionDTO dto)
        {
            if (!IsAdmin()) return Unauthorized();

            await _httpClient.PutAsJsonAsync($"https://localhost:7212/api/Admin/UpdateProfession/{id}", dto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProfession(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            await _httpClient.DeleteAsync($"https://localhost:7212/api/Admin/DeleteProfession/{id}");
            return RedirectToAction("Index");
        }

        // ============= PROFESSION RELATIONS ============

        [HttpPost]
        public async Task<IActionResult> AddRelation(ProfessionRelationDTO dto)
        {
            if (!IsAdmin()) return Unauthorized();

            await _httpClient.PostAsJsonAsync("https://localhost:7212/api/Admin/AddProfessionRelation", dto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRelation(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            await _httpClient.DeleteAsync($"https://localhost:7212/api/Admin/DeleteProfessionRelation/{id}");
            return RedirectToAction("Index");
        }

        // ============= PROFESSION-SKILL BINDINGS =============

        [HttpPost]
        public async Task<IActionResult> AddSkillToProfession(int professionId, int skillId)
        {
            if (!IsAdmin()) return Unauthorized();

            var dto = new ProfessionSkillDTO
            {
                ProfessionId = professionId,
                SkillId = skillId
            };

            await _httpClient.PostAsJsonAsync("https://localhost:7212/api/Admin/AddSkillToProfession", dto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSkillFromProfession(int professionId, int skillId)
        {
            if (!IsAdmin()) return Unauthorized();

            await _httpClient.DeleteAsync($"https://localhost:7212/api/Admin/RemoveSkillFromProfession?professionId={professionId}&skillId={skillId}");
            return RedirectToAction("Index");
        }
    }

}
