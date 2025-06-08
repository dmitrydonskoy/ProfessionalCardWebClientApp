namespace ProfessionalCardWebClientApp.DTO
{
    public class UserProfileDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public List<ProfessionDTO> CurrentProfessions { get; set; } = new();
        public List<FavoriteProfessionDTO> FavoriteProfessions { get; set; } = new();

        public List<SkillDTO> UserSkills { get; set; } = new();

        public List<ProfessionDTO> AvailableProfessions { get; set; } = new();
        public List<SkillDTO> AvailableSkills { get; set; } = new();
    }
}
