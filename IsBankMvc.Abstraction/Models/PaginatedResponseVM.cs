namespace IsBankMvc.Abstraction.Models
{
    public class PaginatedResponseVM<T>
    {
        public T[] Items { get; set; } = [];
        public int Page { get; set; }
        public double PageSize { get; set; }
        public double TotalItems { get; set; }

        public double TotalPages => TotalItems == 0 ? 0 : Math.Ceiling(TotalItems / PageSize);
    }
}
