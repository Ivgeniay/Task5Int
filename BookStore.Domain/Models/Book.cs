namespace BookStore.Domain.Models
{
    public class Book
    {
        public int Index { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<string> Authors { get; set; } = new List<string>();
        public string Publisher { get; set; } = string.Empty;
    }
}
