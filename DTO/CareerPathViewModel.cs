namespace ProfessionalCardWebClientApp.DTO
{
    public class CareerPathViewModel
    {
        public CareerPathDTO CareerPath { get; set; }
        public List<ProfessionDTO> AvailableProfessions { get; set; } = new();
        public int? SelectedProfessionId { get; set; }
    }
}
