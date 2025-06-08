namespace ProfessionalCardWebClientApp.DTO
{
    public class AdminPanelViewModel
    {
        // 🔽 Данные
        public List<SkillDTO> Skills { get; set; } = new();
        public List<ProfessionDTO> Professions { get; set; } = new();
        public List<ProfessionRelationDTO> ProfessionRelations { get; set; } = new();

        // 🔽 Фильтры
        public string SkillSearchTerm { get; set; }
        public string SkillCategoryFilter { get; set; }
        public string ProfessionSearchTerm { get; set; }
        public string ProfessionCategoryFilter { get; set; }
        public string ProfessionLevelFilter { get; set; }
        public string RelationTypeFilter { get; set; }
        public string SourceProfessionFilterName { get; set; }
        public string TargetProfessionFilterName { get; set; }
        public int? SourceProfessionFilter { get; set; }
        public int? TargetProfessionFilter { get; set; }

        // 🔽 Пагинация общая
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // 🔽 Расчётные
        public int TotalSkills => Skills?.Count ?? 0;
        public int TotalProfessions => Professions?.Count ?? 0;
        public int TotalRelations => ProfessionRelations?.Count ?? 0;

        public IEnumerable<SkillDTO> PagedSkills => Skills.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        public IEnumerable<ProfessionDTO> PagedProfessions => Professions.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        public IEnumerable<ProfessionRelationDTO> PagedRelations => ProfessionRelations.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

        // 🔽 МОДЕЛИ ПАГИНАЦИИ
        public PaginationModel SkillPagination => new PaginationModel
        {
            PageNumber = CurrentPage,
            TotalPages = (int)Math.Ceiling((double)TotalSkills / PageSize),
            Tab = "skills"
        };

        public PaginationModel ProfessionPagination => new PaginationModel
        {
            PageNumber = CurrentPage,
            TotalPages = (int)Math.Ceiling((double)TotalProfessions / PageSize),
            Tab = "professions"
        };

        public PaginationModel RelationPagination => new PaginationModel
        {
            PageNumber = CurrentPage,
            TotalPages = (int)Math.Ceiling((double)TotalRelations / PageSize),
            Tab = "relations"
        };
    }
}