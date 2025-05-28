namespace BookStore.Domain.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public bool HasMore { get; set; }
    }
}
