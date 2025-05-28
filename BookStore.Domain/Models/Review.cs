namespace BookStore.Domain.Models
{
    public class Review
    {
        public string Author { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
