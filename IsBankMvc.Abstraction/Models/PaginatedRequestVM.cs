
namespace IsBankMvc.Abstraction.Models
{
    public class PaginatedRequestVM
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Query { get; set; }
    }
}
