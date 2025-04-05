namespace ProfessionalCardWebClientApp.DTO
{
    public class RelationDTO
    {
        public int Id { get; set; }
        public int SourceProfessionId { get; set; }
        public int TargetProfessionId { get; set; }
        public string RelationType { get; set; }
    }
}
