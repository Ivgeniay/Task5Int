using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface IBookGenerationService
    {
        PagedResult<Book> GenerateBooks(GenerationParameters parameters);
        BookDetails GenerateBookDetails(int bookIndex, GenerationParameters parameters);
    }
}
