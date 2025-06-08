namespace ProfessionalCardWebClientApp.DTO
{
    public class ProfessionRelationDTO
    {
        public int Id { get; set; }
        public int SourceProfessionId { get; set; } // ID исходной профессии
        public int TargetProfessionId { get; set; } // ID целевой профессии
        public string RelationType { get; set; } // Тип связи ("равная" или "иерархическая")
        public string SourceProfessionName { get; set; }
        public string TargetProfessionName { get; set; }
    }
}
