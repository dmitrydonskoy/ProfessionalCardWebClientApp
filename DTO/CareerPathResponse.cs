namespace ProfessionalCardWebClientApp.DTO
{
    public class CareerPathResponse
    {
        public List<SkillDTO> JuniorSkills { get; set; } = new();
        public List<SkillDTO> MiddleSkills { get; set; } = new();
        public List<SkillDTO> SeniorSkills { get; set; } = new();
        public List<SkillDTO> ManagerSkills { get; set; } = new();
        public List<AlternativeCareerPathDTO> AlternativePaths { get; set; } = new();
    }
}
