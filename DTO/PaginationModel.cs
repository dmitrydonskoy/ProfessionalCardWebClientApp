namespace ProfessionalCardWebClientApp.DTO
{
    public class PaginationModel
    {
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Tab { get; set; }

        public Dictionary<string, string> Filters { get; set; } = new();

        // 🟢 Добавляем вычисляемые свойства:
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

}
