namespace ProfessionalCardWebClientApp.DTO
{
    public class UserProfileViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string CurrentProfession { get; set; }
        public List<string> FavoriteProfessions { get; set; }
        public List<ProfessionDTO> AvailableProfessions { get; set; }
        public int SelectedFavoriteProfessionId { get; set; }
    }
}
