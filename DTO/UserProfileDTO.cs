namespace ProfessionalCardWebClientApp.DTO
{
    public class UserProfileDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ProfessionDTO? CurrentProfession { get; set; }
   
        public List<ProfessionDTO> FavoriteProfessions { get; set; } = new();
    }
}
