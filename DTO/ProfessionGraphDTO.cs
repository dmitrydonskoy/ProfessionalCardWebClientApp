namespace ProfessionalCardWebClientApp.DTO
{

    public class ProfessionGraphDTO
    {
        public List<ProfessionDTO> Professions { get; set; } = new List<ProfessionDTO>();
        public List<RelationDTO>? Relations { get; set; }  
    }
}
