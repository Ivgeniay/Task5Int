using BookStore.Domain.Models;

namespace BookStore.Infrastructure.Interfaces
{
    public interface ICsvExportService
    {
        string ExportToCsv<T>(IEnumerable<T> data, Region region);
    }
}
