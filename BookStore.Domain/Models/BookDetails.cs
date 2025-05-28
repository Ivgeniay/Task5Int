namespace BookStore.Domain.Models
{
    public class BookDetails
    {
        public Book Book { get; set; }
        public string CoverImageUrl { get; set; } = string.Empty;
        public int Likes { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();

        public BookDetails(Book book) =>
            Book = book ?? throw new ArgumentNullException(nameof(book));
    }
}
