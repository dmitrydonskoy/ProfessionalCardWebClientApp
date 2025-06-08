namespace ProfessionalCardWebClientApp.DTO
{
    public class AlternativeCareerPathDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        public List<SkillDTO> JuniorSkills { get; set; }
        public List<SkillDTO> MiddleSkills { get; set; }
        public List<SkillDTO> SeniorSkills { get; set; }
        public List<SkillDTO> ManagerSkills { get; set; }
    }
}
