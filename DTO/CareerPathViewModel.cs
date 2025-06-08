namespace ProfessionalCardWebClientApp.DTO
{
    public class CareerPathViewModel
    {
        public List<ProfessionDTO> AvailableProfessions { get; set; }
        public int? SelectedProfessionId { get; set; }
        public int? SelectedCurrentProfessionId { get; set; }
        public string? SelectedCurrentProfessionLevel { get; set; }
        public string? SelectedCurrentProfessionName { get; set; } = null;
        public CareerPathResponse CareerPath { get; set; }
        public bool IsPersonalizedComparison { get; set; } = false;
    }
}
